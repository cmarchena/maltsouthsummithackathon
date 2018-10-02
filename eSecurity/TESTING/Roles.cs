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
    /// Clase de pruebas unitarias para la entidad Roles
    /// </summary>
    [TestClass]
    public class Roles
    {
        private SECURITY.CORE.Entities.Roles Store { get; set; }

        public Roles()
        {
            Store = new SECURITY.CORE.Entities.Roles
            {
                UserStore = new UserManager<User>(new UserStore<User>(new DbContext()))
                {
                    DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(20),
                    MaxFailedAccessAttemptsBeforeLockout = 2
                },

                RoleStore = new RoleManager<Role>(new RoleStore<Role>(new DbContext()))
            };
        }

        [TestMethod]
        [DataRow("f8b11ebc-e8db-44b5-adc9-71d2c6fe1e35")]
        public async Task GetAsync(string internalId)
        {
            try
            {
                var r = await Store.Get(Guid.Parse(internalId));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }            
        }
    }
}
