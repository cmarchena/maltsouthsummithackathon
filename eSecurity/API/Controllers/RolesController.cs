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
    /// Controlador encargado de gestionar los Roles
    /// </summary>
    [RoutePrefix("{lang}/api/v1.0")]
    public class RolesController : ApiController
    {
        #region "Constructores"
        protected readonly IRoles Store;

        public RolesController(IRoles store)
        {
            Store = store;
            Store.User = User;
            Store.RoleStore = new RoleManager<Role>(new RoleStore<Role>(new DbContext()));
            Store.UserStore = new UserManager<User>(new UserStore<User>(new DbContext()));
        }
        #endregion

        #region "Get"
        /// <summary>
        /// Devuelve un rol por su Identificador
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("roles/{roleId:Guid}")]
        [ResponseType(typeof(User))]        
        async public Task<IHttpActionResult> Get(Guid roleId)
        {
            try
            {
                var results = await Store.Get(roleId);

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
        /// Crea un Rol
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("roles")]
        [ResponseType(typeof(IdentityResult))]
        async public Task<IHttpActionResult> Post(Role model)
        {
            try
            {
                var results = await Store.Post(model);

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

        #region "GetUsersInRole"
        /// <summary>
        /// Devuelve una lista de usuarios que contengan el rol
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("roles/users/inrole/{name}")]
        [ResponseType(typeof(List<string>))]
        async public Task<IHttpActionResult> GetUsersInRole(string name)
        {
            try
            {
                var results = await Store.GetUsersInRole(name);

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

        #region "IsInRole"

        /// <summary>
        /// Devuelve true si el usuario pertenece al rol indicado
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [Authorization(AppSettingRoles = "roles:administracion")]
        [Route("roles/user/{userName}/isinrole/{roleName}")]
        [ResponseType(typeof(bool))]
        async public Task<IHttpActionResult> IsInRole(string userName, string roleName)
        {
            try
            {
                var results = await Store.IsInRole(userName, roleName);

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

    }
}
