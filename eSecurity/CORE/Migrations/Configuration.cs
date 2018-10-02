using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;

using SECURITY.CORE.Entities;
using SECURITY.MODEL.Entities;

namespace SECURITY.CORE.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<Globals.DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            if (ConfigurationManager.AppSettings["set:entity:generators"] != null)
            {
                DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
                SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
            }
        }

        async protected override void Seed(Globals.DbContext context)
        {
            var startup = new Startups
            {
                RoleStore = new RoleManager<Role>(new RoleStore<Role>(new Globals.DbContext())),
                UserStore = new UserManager<User>(new UserStore<User>(new Globals.DbContext()))
            };

            await startup.Init();
        }
    }
}
