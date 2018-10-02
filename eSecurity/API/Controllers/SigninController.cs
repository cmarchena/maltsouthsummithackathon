using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

using SECURITY.API.Globals;
using SECURITY.API.Globals.Identity;

namespace SECURITY.API.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar los Signin
    /// </summary>
    public class SigninController : ApiController
    {

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [SwaggerConfig.SwaggerFormatValue("password", "password")]
        [SwaggerConfig.SwaggerFormatValue("audienceId", "password")]
        public async Task<IHttpActionResult> Login(string userName, string password, string audienceId, bool isPersistent)
        {
            var signInManager = HttpContext.Current.GetOwinContext().Get<SignInManager>();
            var userManager = signInManager.UserManager;
            var authenticationManager = signInManager.AuthenticationManager;

            var response = new Response(true, HttpStatusCode.OK);

            var signinStatus = await signInManager.PasswordSignInAsync(userName, password, isPersistent, true);

            if (signinStatus != SignInStatus.Success)
            {
                response.Success = true;
                response.Status.Code = HttpStatusCode.Unauthorized;
                response.Status.Message = RESOURCES.Core.invalidGrant;
            }
            else {
                var user = await userManager.FindAsync(userName, password);

                ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(user, "JWT");

                AuthenticationProperties oAuthProperties = new AuthenticationProperties
                {
                    IssuedUtc = DateTime.Now,
                    ExpiresUtc = DateTime.Now.AddMinutes(20),
                };

                oAuthProperties.Dictionary.Add("ClientId", user.InternalId);
                oAuthProperties.Dictionary.Add("AudienceId", audienceId);

                var ticket = new AuthenticationTicket(oAuthIdentity, oAuthProperties);

                var token = Globals.Security.Token.Generate(ticket);

                response.Success = true;
                response.Status.Code = HttpStatusCode.OK;
                response.Status.Message = token;
            }

            return Json(new
            {
                response
            });

        }
    }
}
