using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

using SECURITY.MODEL.Entities;

namespace SECURITY.MODEL.Interfaces
{
    /// <summary>
    /// Define el Interface para la gestión de Usuarios
    /// </summary>
    public interface IUsers
    {
        #region "Miembros"

        UserManager<User> UserStore { get; set; }

        IPrincipal User { get; set; }
        #endregion

        #region "GetById"
        /// <summary>
        /// Devuelve <see cref="User"/> cuyo internalId coincida
        /// </summary>
        /// <param name="internalId">Key</param>
        /// <returns>{User}</returns>
        Task<User> Get(Guid internalId);
        #endregion

        #region "Create"
        /// <summary>
        /// Crea un <see cref="User"/>
        /// </summary>
        /// <param name="model">{User}</param>
        /// <returns>{User}</returns>
        Task<IdentityResult> Post(User model, string password);
        #endregion

        #region "Update"
        /// <summary>
        /// Actualiza un <see cref="User"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns>{User}</returns>
        Task<IdentityResult> Put(User model);
        #endregion

        #region "Delete"
        /// <summary>
        /// Elimina una <see cref="User"/>
        /// </summary>
        /// <param name="internalId"></param>
        /// <returns>{User}</returns>
        Task<IdentityResult> Delete(Guid internalId);
        #endregion

        #region "Change Password"
        Task<IdentityResult> ChangePassword(Guid internalId, string currentPassword, string newPassword);
        #endregion

        #region "Check Password"
        Task<bool> CheckPassword(string userName, string password);
        #endregion

        #region "Roles"

        #region "IsInRole"
        /// <summary>
        /// Devuelve true si el usuario contiene el rol
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="role">Rol1</param>
        /// <returns></returns>
        Task<bool> IsInRole(string userName, string role);
        #endregion

        #region "GetRoles"
        /// <summary>
        /// Devuelve los roles del usuario
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <returns></returns>
        Task<IList<string>> GetRoles(string userName);
        #endregion

        #region "AddToRole"
        /// <summary>
        /// Añade al usuario al rol
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="role">Rol1</param>
        /// <returns></returns>
        Task<User> AddToRole(string userName, string role);
        #endregion

        #region "AddToRoles"
        /// <summary>
        /// Añade al usuario a la lista de roles separados por comas
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="roles">Rol1,Rol2,Rol3</param>
        /// <returns></returns>
        Task<User> AddToRoles(string userName, string roles);
        #endregion


        #region "RemoveFromRole"
        /// <summary>
        /// Quita al usuario del rol
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="role">Rol1</param>
        /// <returns></returns>
        Task<User> RemoveFromRole(string userName, string role);
        #endregion

        #region "RemoveFromRoles"
        /// <summary>
        /// Quita al usuario del rol
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="roles">Rol1,Rol2,Rol3</param>
        /// <returns></returns>
        Task<User> RemoveFromRoles(string userName, string roles);
        #endregion

        #endregion

    }
}

