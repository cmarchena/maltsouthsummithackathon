using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;

using ePassport.API.Globals.Utilities;

namespace ePassport.API.Controllers
{
    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {
        private object Utilities;
        #region "Token"
        /// <summary>
        /// Autoriza al usuario a utilizar el API con sus credenciales y permisos
        /// </summary>
        /// <param name="username">Nombre Usuario</param>
        /// <param name="password">Password</param>
        /// <param name="audienceId">Id Cliente</param>
        /// <param name="grantType">Tipo de permiso</param>
        /// <returns></returns>
        [Route("{audienceId}/{granttype}/{username}/{password}")]
        [SwaggerConfig.SwaggerFormatValue("audienceId", "password")]
        [SwaggerConfig.SwaggerFormatValue("password", "password")]
        [SwaggerConfig.SwaggerDefaultValue("grantType", "password")]
        async public Task<IHttpActionResult> Token(string username, string password, string audienceId, string grantType)
        {
            try
            {
                var client = new ClientHttp(ConfigurationManager.AppSettings["issuer"])
                {
                    Content = "username=" + username + "&password=" + password + "&grant_type=" + grantType,
                    Action = "token?audienceId=" + audienceId
                };

                await client.Post();

                if (client.Response["access_token"] != null)
                {
                    SwaggerConfig.AccessToken = client.Response["token_type"] + " " + client.Response["access_token"];
                }

                if (client.Response["error"] != null)
                {
                    return Json(new
                    {
                        error = client.Response["error"],
                        error_description = client.Response["error_description"]
                    });
                }

                return Json(new
                {
                    access_token = client.Response["access_token"],
                    refresh_token = client.Response["refresh_token"],
                    token_type = client.Response["token_type"],
                    expires_in = client.Response["expires_in"],
                    issued = Globals.Utilities.DateTime.ToLocalTime(client.Response[".issued"].ToString()),
                    expires = Globals.Utilities.DateTime.ToLocalTime(client.Response[".expires"].ToString()),
                    message = ""
                });

            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = "KO"
                });
            }
        }
        #endregion

        #region "Token Refresh"
        /// <summary>
        /// Autoriza al usuario a utilizar el API con sus credenciales y permisos
        /// </summary>
        /// <param name="username">Nombre Usuario</param>
        /// <param name="refresh_token">Token de refresco</param>
        /// <param name="grantType">Tipo de permiso</param>
        /// <returns></returns>
        [SwaggerConfig.SwaggerDefaultValue("grantType", "password")]/// 
        [Route("token/refresh/{granttype}/{username}/{refresh_token}")]
        async public Task<IHttpActionResult> RefreshToken(string username, string refresh_token, string grantType)
        {
            try
            {
                var client = new ClientHttp(ConfigurationManager.AppSettings["issuer"]);

                client.Content = "username=" + username + "&refresh_token=" + refresh_token + "&grant_type=" + grantType;
                client.Action = "token";

                await client.Post();

                if (client.Response["access_token"] != null)
                {
                    SwaggerConfig.AccessToken = client.Response["token_type"] + " " + client.Response["access_token"];
                }

                if (client.Response["error"] != null)
                {
                    return Json(new
                    {
                        error = client.Response["error"],
                        error_description = client.Response["error_description"]
                    });
                }

                return Json(new
                {
                    access_token = client.Response["access_token"],
                    refresh_token = client.Response["refresh_token"],
                    token_type = client.Response["token_type"],
                    expires_in = client.Response["expires_in"],
                    issued = Globals.Utilities.DateTime.ToLocalTime(client.Response[".issued"].ToString()),
                    expires = Globals.Utilities.DateTime.ToLocalTime(client.Response[".expires"].ToString()),
                    message = ""
                });

            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = "KO"
                });
            }
        }
        #endregion     
    }
}
