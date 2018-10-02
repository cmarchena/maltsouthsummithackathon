using System.Configuration;
using Owin;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;

namespace ePassport.API
{
    /// <summary>
    /// Clase que maneja el consumo de tokens JWT de seguridad
    /// </summary>
    public static class TokenConsumptionConfig
    {
        public static void Register(IAppBuilder app)
        {
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Todos los API controller con el atributo [Authorize] serán validados por JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityKeyProviders = new IIssuerSecurityKeyProvider[] {
                        new SymmetricKeyIssuerSecurityKeyProvider(ConfigurationManager.AppSettings["issuer"], audienceSecret)
                    }                    
                });
        }
    }
}
