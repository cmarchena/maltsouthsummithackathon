using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using Unity;
using Unity.Exceptions;

namespace ePassport.API.Globals
{
    namespace Dependency
    {

        public class Resolver : IDependencyResolver
        {
            protected IUnityContainer container;

            public Resolver(IUnityContainer container)
            {
                this.container = container ?? throw new ArgumentNullException("container");
            }

            public object GetService(Type serviceType)
            {
                try
                {
                    return container.Resolve(serviceType);
                }
                catch (ResolutionFailedException)
                {
                    return null;
                }
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                try
                {
                    return container.ResolveAll(serviceType);
                }
                catch (ResolutionFailedException)
                {
                    return new List<object>();
                }
            }

            public IDependencyScope BeginScope()
            {
                var child = container.CreateChildContainer();
                return new Resolver(child);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
                container.Dispose();
            }
        }
    }

    namespace Utilities
    {
        public static class DateTime
        {
            public static System.DateTime? ToLocalTime(string date)
            {
                if (date != string.Empty && date != null)
                {
                    TimeZone zone = TimeZone.CurrentTimeZone;
                    System.DateTime local = zone.ToLocalTime(System.DateTime.Parse(date));
                    return local;
                }
                return null;
            }

            public static System.DateTime? ToUniversalTime(string date)
            {
                if (date != string.Empty && date != null)
                {
                    TimeZone zone = TimeZone.CurrentTimeZone;
                    System.DateTime universal = zone.ToUniversalTime(System.DateTime.Parse(date));
                    return universal;
                }
                return null;
            }


        }
    }

    namespace Security
    {
        /// <summary>
        /// Atributo para poder gestionar la seguridad de las acciones respecto a appsettings del web.config
        /// TODO:Estas settings habría que pasarlas a bbdd por ejemplo a la tabla de configuraciones
        /// </summary>
        public class AuthorizationAttribute : AuthorizeAttribute
        {
            public string AppSettingRoles { get; set; }
            public bool Authorized { get; set; }
            public ClaimsPrincipal Principal { get; set; }

            /// <summary>
            /// Crea la Identidad desde el token, si pasa la seguridad
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            private void SetPrincipal(string token)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                    var symmetricKey = Convert.FromBase64String(ConfigurationManager.AppSettings["as:AudienceSecret"]);

                    var validationParameters = new TokenValidationParameters()
                    {
                        RequireExpirationTime = true,
                        ValidIssuer = ConfigurationManager.AppSettings["issuer"],
                        ValidAudience = ConfigurationManager.AppSettings["as:AudienceId"],
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                    };
                    
                    Principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

                }
                catch (Exception e)
                {
                    Principal = null;
                }
            }

            /// <summary>
            /// Proceso de autorización
            /// </summary>
            /// <param name="actionContext"></param>
            public override void OnAuthorization(HttpActionContext actionContext)
            {
                try
                {
                    Authorized = false;

                    var token = actionContext.Request.Headers.Authorization.Parameter;

                    SetPrincipal(token);

                    if (Principal != null && Principal.Identity.IsAuthenticated)
                    {
                        Roles = ConfigurationManager.AppSettings[this.AppSettingRoles];
                        actionContext.ControllerContext.RequestContext.Principal = Principal;
                        actionContext.RequestContext.Principal = Principal;

                        if (Roles != null && Roles != string.Empty)
                        {
                            foreach (var role in Roles.Split(','))
                            {
                                if (Principal.IsInRole(role))
                                {
                                    Authorized = true;
                                    break;
                                }
                            }

                            if (!Authorized) actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        }
                    }
                    else
                    {
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }
                }
                catch (Exception e)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }

            // This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.
            protected virtual bool AuthorizeCore(HttpContextBase httpContext)
            {
                if (httpContext == null)
                {
                    throw new ArgumentNullException("httpContext");
                }

                IPrincipal user = httpContext.User;
                if (!user.Identity.IsAuthenticated)
                {
                    return false;
                }

                return true;
            }
        }
    }
}