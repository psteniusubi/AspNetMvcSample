using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AspNetMvcSample
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

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

            app.UseOpenIdConnectAuthentication(openIdConnectOptions);
        }
    }
}
