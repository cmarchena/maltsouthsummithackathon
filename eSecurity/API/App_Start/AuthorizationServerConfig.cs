using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SECURITY.API.Globals.Identity;
using SECURITY.API.Globals.Identity.Providers;
using System;
using System.Configuration;

namespace SECURITY.API
{
    public class AuthorizationServerConfig
    {
        public static void Register(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = bool.Parse(ConfigurationManager.AppSettings["AllowInsecureHttp"]),
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(20),
                AccessTokenFormat = new AccessTokenFormat(),
                Provider = new OAuthProvider(),
                RefreshTokenProvider = new RefreshTokenProvider()
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
        }
    }
}