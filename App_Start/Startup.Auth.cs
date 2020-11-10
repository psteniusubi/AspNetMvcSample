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

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    Authority = ConfigurationManager.AppSettings.Get("issuer"),
                    ClientId = ConfigurationManager.AppSettings.Get("client_id"),
                    ClientSecret = ConfigurationManager.AppSettings.Get("client_secret"),
                    ResponseType = "code", // authorization code flow
                    ResponseMode = null, // leave undefined, defaults to query
                    Scope = "openid", // enables openid connect
                    RedirectUri = ConfigurationManager.AppSettings.Get("redirect_uri"),
                    RedeemCode = true, // authorization code flow
                    ProtocolValidator = new OpenIdConnectProtocolValidator
                    {
                        RequireStateValidation = false, // TODO: check why this is needed
                        RequireNonce = false, // TODO: check why this is needed
                    }
                });
        }
    }
}