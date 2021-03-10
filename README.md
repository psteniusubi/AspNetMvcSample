# ASP.NET MVC and Ubisecure SSO integration with OpenID Connect

## Introduction

This is a sample ASP.NET MVC application to illustrate integration with OpenID Connect Auhthorization Code flow.

The [AspNetCoreSample](../../../AspNetCoreSample) project has a version for ASP.NET Core.

## Configuration

An OpenID Connect Client needs to be configured with information about the OpenID Connect Provider and client credentials. This sample app puts these configuration items into [web.config](Web.config) file as AppSettings keys:

* `issuer` - name of OpenID Connect Provider
* `client_id` and `client_secret` - client credentials registered with OpenID Connect Provider 
* `redirect_uri` - this value must match deployment and is registered with OpenID Connect Provider

## Code review

This is a simplified ASP.NET MVC web app. Only the minimum required files are included. Most important being

* [App_Start/Startup.Auth.cs](App_Start/Startup.Auth.cs)
* [Controllers/HomeController.cs](Controllers/HomeController.cs)
* [Views/Home/Index.cshtml](Views/Home/Index.cshtml)

### Startup.Auth.cs

Purpose of this class is to setup authentication related middleware. 

This enables generic ASP.NET MVC cookie based session tracking. 

```c#
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
```

Next code sets up the OpenID Connect middleware. I'm reading settings from `web.config` and assiging values to [OpenIdConnectAuthenticationOptions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.owin.security.openidconnect.openidconnectauthenticationoptions) properties. 

* `ResponseType = "code"` lets me use Authorization Code Flow
* `ResponseMode = null` defaults to query, which tells provider to use HTTP GET method when redirecting back
* `Scope = "openid"` enables OpenID Connect. Add more scope values if you need access to more services
* `RedeemCode = true` makes sure OpenID Connect middleware performs Authorization Code Flow

```c#
            var redirectUri = new Uri(ConfigurationManager.AppSettings.Get("redirect_uri"));
            var openIdConnectOptions = new OpenIdConnectAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings.Get("issuer"),
                ClientId = ConfigurationManager.AppSettings.Get("client_id"),
                ClientSecret = ConfigurationManager.AppSettings.Get("client_secret"),
                ResponseType = "code", // authorization code flow
                ResponseMode = null, // leave undefined, defaults to query
                Scope = "openid", // enables openid connect
                RedirectUri = redirectUri.OriginalString,
                RedeemCode = true, // authorization code flow
            };
```

The following is a workaround for https://github.com/aspnet/AspNetKatana/issues/386. Please remove if your app is always deployed to encrypted https host, as this issue only occurs when running on plain http host. 

The Visual Studio project of this sample app is setup to run on http://localhost:52834/ which is fine for a development experience but needs this workaround.

```c#
            // the following is a workaround for https://github.com/aspnet/AspNetKatana/issues/386
            // make sure to only enable when running on localhost without https
            if (!"https".Equals(redirectUri.Scheme) && redirectUri.IsLoopback)
            {
                openIdConnectOptions.ProtocolValidator = new OpenIdConnectProtocolValidator
                {
                    RequireStateValidation = false,
                    RequireNonce = false,
                };
            }
```

This final piece injects OpenID Connect middleware into the pipeline of the ASP.NET MVC app.

```c#
            app.UseOpenIdConnectAuthentication(openIdConnectOptions);
```

### HomeController.cs

`HomeController` has a single operation that sets the model to current user. `[Authorize]` tag tells the ASP.NET middleware that access to this controller requires authentication.

```c#
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(User as ClaimsPrincipal);
        }
    }
```

### Index.cshtml

The following generates a simple html list showing all [claims](https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal) received from OpenID Connect provider

```cshtml
@model System.Security.Claims.ClaimsPrincipal

<!DOCTYPE html>

<html>
<body>
    <h1>Welcome</h1>
    <dl>
        @foreach (var claim in Model.Claims)
        {
            <dt><b>@claim.Type</b></dt>
            <dd><i>@claim.Value</i></dd>
        }
    </dl>
</body>
</html>
```

## Launching

Load the project into Visual Studio 2019 and run. The `web.config` file is ready configured to access Ubisecure SSO at https://login.example.ubidemo.com/uas.

This app is also deployed live on https://ubi-aspnet-mvc-sample.azurewebsites.net.

Login with your Email, Google, Facebook or Microsoft account.
