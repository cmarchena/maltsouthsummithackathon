using System.Web.Http;
using Unity;
using Unity.Lifetime;

using SECURITY.API.Globals.Dependency;
using SECURITY.CORE.Entities;
using SECURITY.MODEL.Interfaces;

namespace SECURITY.API
{
    /// <summary>
    /// Clase que maneja el registro de dependencias a través de un contenedor de Unity
    /// </summary>
    public static class DependencyConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();

            //Aquí hay que registrar los tipos en el contenedor Unity para que pueda inyectarlos
            container.RegisterType<IAudiences, Audiences>(new HierarchicalLifetimeManager());
            container.RegisterType<IRoles, Roles>(new HierarchicalLifetimeManager());
            container.RegisterType<IUsers, Users>(new HierarchicalLifetimeManager());

            config.DependencyResolver = new Resolver(container);
        }
    }
}