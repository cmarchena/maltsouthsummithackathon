using System.Web.Http;
using Unity;
using Unity.Lifetime;

using ePassport.API.Globals.Dependency;
using ePassport.CORE.Interfaces;
using ePassport.CORE.Entities;

namespace ePassport.API
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
            container.RegisterType<IUsuarios, Usuarios>(new HierarchicalLifetimeManager());


            config.DependencyResolver = new Resolver(container);
        }


    }
}
