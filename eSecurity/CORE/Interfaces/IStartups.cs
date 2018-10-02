using Microsoft.AspNet.Identity;
using System.Security.Principal;
using System.Threading.Tasks;

using SECURITY.MODEL.Entities;

namespace SECURITY.MODEL.Interfaces
{
    /// <summary>
    /// Define el Interface para la gestión de Startups
    /// </summary>
    public interface IStartups
    {
        #region "Miembros"

        RoleManager<Role> RoleStore { get; set; }
        UserManager<User> UserStore { get; set; }

        IPrincipal User { get; set; }
        #endregion

        #region "Init"
        /// <summary>
        /// Creará los roles y usuarios de las configuraciones startup del web.config
        /// </summary>
        /// <returns></returns>
        Task<bool> Init();
        #endregion

    }
}

