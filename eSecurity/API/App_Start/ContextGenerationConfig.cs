using Owin;

using SECURITY.API.Globals.Identity;
using SECURITY.CORE.Globals;

namespace SECURITY.API
{
    /// <summary>
    /// Clase que maneja la generación de los contextos
    /// </summary>
    public static class ContextGenerationConfig
    {
        public static void Register(IAppBuilder app)
        {
            app.CreatePerOwinContext(DbContext.Create);
            app.CreatePerOwinContext<UserManager>(UserManager.Create);
            app.CreatePerOwinContext<RoleManager>(RoleManager.Create);
            app.CreatePerOwinContext<SignInManager>(SignInManager.Create);
        }
    }
}