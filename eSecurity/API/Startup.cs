using Microsoft.Owin;
using Microsoft.Owin.Cors;
using MySql.Data.Entity;
using Owin;
using System.Data.Entity;

[assembly: OwinStartup(typeof(SECURITY.API.Startup))]

namespace SECURITY.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Para obtener más información sobre cómo configurar la aplicación, visite https://go.microsoft.com/fwlink/?LinkID=316888

            //JWT
            //TokenConsumptionConfig.Register(app);

            ContextGenerationConfig.Register(app);

            AuthorizationServerConfig.Register(app);

            //CORS
            app.UseCors(CorsOptions.AllowAll);

            //DbConfiguration
            DbConfiguration.SetConfiguration(new MySqlEFConfiguration());
        }
    }
}
