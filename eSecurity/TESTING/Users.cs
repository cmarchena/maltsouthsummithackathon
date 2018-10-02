using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SECURITY.CORE.Globals;
using SECURITY.MODEL.Entities;

namespace TESTING
{
    /// <summary>
    /// Clase de pruebas unitarias para la entidad Users
    /// </summary>
    [TestClass]
    public class Users
    {
        private SECURITY.CORE.Entities.Users Store {get;set;}

        public Users()
        {
            Store = new SECURITY.CORE.Entities.Users
            {
                UserStore = new UserManager<User>(new UserStore<User>(new DbContext()))
                {
                    DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(20),
                    MaxFailedAccessAttemptsBeforeLockout = 2
                }
            };
        }

        /// <summary>
        /// Verifica si el usuario ha introducido la password adecuada
        /// </summary>
        /// <param name="userName">Nombre de Usuario</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        [TestMethod]
        [DataRow("Usuario","Usuario")]
        [DataRow("Administrador", "Necleman2007")]
        public async Task CheckPasswordAsync(string userName, string password)
        {
            try
            {
                var r = await Store.CheckPassword(userName, password);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}
