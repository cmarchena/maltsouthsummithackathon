using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

using SECURITY.MODEL.Entities;

namespace SECURITY.MODEL.Interfaces
{
    /// <summary>
    /// Define el Interface para la gestión de Roles
    /// </summary>
    public interface IRoles
    {
        #region "Miembros"

        RoleManager<Role> RoleStore { get; set; }
        UserManager<User> UserStore { get; set; }

        IPrincipal User { get; set; }
        #endregion

        #region "GetById"
        /// <summary>
        /// Devuelve <see cref="Role"/> cuyo internalId coincida
        /// </summary>
        /// <param name="internalId">Key</param>
        /// <returns>{User}</returns>
        Task<Role> Get(Guid internalId);
        #endregion

        #region "GetByName"
        /// <summary>
        /// Devuelve <see cref="Role"/> cuyo nombre coincida
        /// </summary>
        /// <param name="role">Key</param>
        /// <returns>{User}</returns>
        Task<Role> Get(string name);
        #endregion

        #region "Create"
        /// <summary>
        /// Crea un <see cref="Role"/>
        /// </summary>
        /// <param name="model">{User}</param>
        /// <returns>{User}</returns>
        Task<IdentityResult> Post(Role model);
        #endregion

        #region "Update"
        /// <summary>
        /// Actualiza un <see cref="Role"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns>{User}</returns>
        Task<IdentityResult> Put(Role model);
        #endregion

        #region "Delete"
        /// <summary>
        /// Elimina una <see cref="Role"/>
        /// </summary>
        /// <param name="internalId"></param>
        /// <returns>{User}</returns>
        Task<IdentityResult> Delete(Guid internalId);
        #endregion
        
        #region "Users In Role"
        /// <summary>
        /// Devuelve a todos los usuarios del rol
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IList<string>> GetUsersInRole(string name);
        #endregion

        #region "Is In Role"
        /// <summary>
        /// Devuelve true si el usuario pertenece al rol
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> IsInRole(string userName,string roleName);
        #endregion
    }
}

