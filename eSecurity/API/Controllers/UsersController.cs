using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

using SECURITY.API.Globals;
using SECURITY.API.Globals.Security;
using SECURITY.CORE.Globals;
using SECURITY.MODEL.Entities;
using SECURITY.MODEL.Interfaces;

namespace SECURITY.API.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar a los Usuarios
    /// </summary>
    [RoutePrefix("{lang}/api/v1.0")]
    public class UsersController : ApiController
    {
        #region "Constructores"
        protected readonly IUsers Store;

        public UsersController(IUsers store)
        {
            Store = store;
            Store.User = User;

            Store.UserStore = new UserManager<User>(new UserStore<User>(new DbContext()))
            {
                DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(20),
                MaxFailedAccessAttemptsBeforeLockout = 2
            };
        }
        #endregion

        #region "Get"
        /// <summary>
        /// Devuelve un usuario por su Identificador Interno de usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("users/{userId:Guid}")]
        [ResponseType(typeof(User))]
        async public Task<IHttpActionResult> Get(Guid userId)
        {
            try
            {
                var results = await Store.Get(userId);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "Post"
        /// <summary>
        /// Crea al usuario
        /// </summary>
        /// <param name="model"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("users")]
        [ResponseType(typeof(IdentityResult))]
        [SwaggerConfig.SwaggerFormatValue("password", "password")]
        async public Task<IHttpActionResult> Post(User model, string password)
        {
            try
            {
                var results = await Store.Post(model, password);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "Put"
        /// <summary>
        /// Actualiza al usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("users")]
        [ResponseType(typeof(IdentityResult))]
        async public Task<IHttpActionResult> Put(User model)
        {
            try
            {
                var results = await Store.Put(model);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "Delete"
        /// <summary>
        /// Elimina al usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("users/{userId:Guid}")]
        [ResponseType(typeof(IdentityResult))]
        async public Task<IHttpActionResult> Delete(Guid userId)
        {
            try
            {
                var results = await Store.Delete(userId);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "ChangePassword"
        /// <summary>
        /// Cambia la password del usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("users/change/password/{userId:Guid}")]
        [ResponseType(typeof(IdentityResult))]
        [SwaggerConfig.SwaggerFormatValue("currentPassword", "password")]
        [SwaggerConfig.SwaggerFormatValue("newPassword", "password")]
        async public Task<IHttpActionResult> ChangePassword(Guid userId, string currentPassword, string newPassword)
        {
            try
            {
                var results = await Store.ChangePassword(userId, currentPassword, newPassword);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "CheckPassword"
        /// <summary>
        /// Verifica la password del usuario
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("users/check/password/{userName}/{password}")]
        [ResponseType(typeof(bool))]
        [SwaggerConfig.SwaggerFormatValue("password", "password")]
        async public Task<IHttpActionResult> CheckPassword(string username, string password)
        {
            try
            {
                var results = await Store.CheckPassword(username, password);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "Roles"

        #region "IsInRole"
        /// <summary>
        /// Devuelve true si el usuario pertenece al rol
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [Authorization()]
        [Route("users/{userName}/isinrole/{role}")]
        [ResponseType(typeof(bool))]
        async public Task<IHttpActionResult> IsInRole(string userName, string role)
        {
            try
            {
                var results = await Store.IsInRole(userName, role);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "GetRoles"
        /// <summary>
        /// Devuelve todos los roles del usuario
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Authorization()]
        [Route("users/{userName}/getroles")]
        [ResponseType(typeof(IList<string>))]
        async public Task<IHttpActionResult> GetRoles(string userName)
        {
            try
            {
                var results = await Store.GetRoles(userName);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "AddToRole"
        /// <summary>
        /// Añade al usuario al rol
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("users/{userName}/role/add/{role}")]
        [ResponseType(typeof(User))]
        async public Task<IHttpActionResult> AddToRole(string userName, string role)
        {
            try
            {
                var results = await Store.AddToRole(userName, role);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "AddToRoles"
        /// <summary>
        /// Añade al usuario a los roles delimitados por comas
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="roles">Rol1,Rol2,Rol3</param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("users/{userName}/roles/add/{roles}")]
        [ResponseType(typeof(User))]
        async public Task<IHttpActionResult> AddToRoles(string userName, string roles)
        {
            try
            {
                var results = await Store.AddToRoles(userName, roles);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "RemoveFomRole"
        /// <summary>
        /// Quita al usuario del rol
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("users/{userName}/role/remove/{role}")]
        [ResponseType(typeof(User))]
        async public Task<IHttpActionResult> RemoveFromRole(string userName, string role)
        {
            try
            {
                var results = await Store.RemoveFromRole(userName, role);
                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #region "RemoveFromRoles"
        /// <summary>
        /// Quita al usuario a los roles delimitados por comas
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="roles">Rol1,Rol2,Rol3</param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("users/{userName}/roles/remove/{roles}")]
        [ResponseType(typeof(User))]
        async public Task<IHttpActionResult> RemoveFromRoles(string userName, string roles)
        {
            try
            {
                var results = await Store.RemoveFromRoles(userName, roles);

                var response = new Response(true, HttpStatusCode.OK);

                return Json(new
                {
                    results,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion


        #region "GetRefreshToken"
        /// <summary>
        /// Obtiene el token de refresco
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("users/get/refresh/token/{userName}/{password}")]
        [ResponseType(typeof(bool))]
        [SwaggerConfig.SwaggerFormatValue("password", "password")]
        async public Task<IHttpActionResult> GetRefreshToken(string username, string password)
        {
            try
            {
                var user = await Store.UserStore.FindAsync(username, password);

                var response = new Response(true, HttpStatusCode.OK);

                if (user != null)
                {
                    if (user.RefreshTokenExpiresUtc == null || Globals.Utilities.DateTime.ToLocalTime(user.RefreshTokenExpiresUtc.ToString()) <= DateTime.Now)
                    {
                        user.RefreshToken = Guid.NewGuid().ToString();
                        user.RefreshTokenIssuedUtc = DateTime.Now;
                        user.RefreshTokenExpiresUtc = DateTime.Now.AddMinutes(20);

                        await Store.UserStore.UpdateAsync(user);
                    }
                }
                else
                {
                    response.Success = true;
                    response.Status.Code = HttpStatusCode.Unauthorized;
                    response.Status.Message = RESOURCES.Core.invalidGrant;
                }

                return Json(new
                {
                    user.RefreshToken,
                    success = response.Success
                });
            }
            catch (Exception e)
            {
                var response = new Response(false, HttpStatusCode.BadRequest, e.Message);

                return Json(new
                {
                    success = response.Success,
                    response.Status.Code,
                    response.Status.Message
                });
            }
        }
        #endregion

        #endregion

    }
}
