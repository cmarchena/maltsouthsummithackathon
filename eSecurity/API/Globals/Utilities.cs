using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using Unity;
using Unity.Exceptions;

using SECURITY.CORE.Globals;
using SECURITY.MODEL.Entities;
using SECURITY.MODEL;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SECURITY.API.Globals
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

    namespace Identity
    {
        /// <summary>
        /// Gestiona los roles
        /// </summary>
        public class RoleManager : RoleManager<IdentityRole>
        {
            public RoleManager(IRoleStore<IdentityRole, string> roleStore)
                : base(roleStore)
            {
            }

            public static RoleManager Create(IdentityFactoryOptions<RoleManager> options, IOwinContext context)
            {
                var appDbContext = context.Get<DbContext>();
                var appRoleManager = new RoleManager(new RoleStore<IdentityRole>(appDbContext));

                return appRoleManager;

            }
        }

        /// <summary>
        /// Gestiona los usuarios
        /// </summary>
        public class UserManager : UserManager<User>
        {
            public UserManager(IUserStore<User> store)
                : base(store)
            {
                
            }

            public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
            {
                var appDbContext = context.Get<DbContext>();
                var appUserManager = new UserManager(new UserStore<User>(appDbContext));

                //todo: Configurar como settings del web.config las políticas y tiempo de activación

                #region "ejemplos customizados"
                //Validadores customizados
                //Políticas de usuario
                //appUserManager.UserValidator = new MyCustomUserValidator(appUserManager)
                //{
                //    AllowOnlyAlphanumericUserNames = true,
                //    RequireUniqueEmail = true
                //};

                //Políticas de password
                //appUserManager.PasswordValidator = new MyCustomPasswordValidator
                //{
                //    RequiredLength = 5,
                //    RequireNonLetterOrDigit = false,
                //    RequireDigit = false,
                //    RequireLowercase = false,
                //    RequireUppercase = false
                //};
                #endregion

                //Políticas de usuario
                appUserManager.UserValidator = new UserValidator<User>(appUserManager)
                {
                    AllowOnlyAlphanumericUserNames = true,
                    RequireUniqueEmail = true                    
                };

                //Máximo de intentos fallidos antes de bloquear la cuenta
                appUserManager.MaxFailedAccessAttemptsBeforeLockout = 5;

                //Bloquea al usuario durante 10 minutos
                appUserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(10);

                //Políticas de password
                appUserManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 5,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false
                };

                //Servicio de email
                //appUserManager.EmailService = new EmailService();

                var dataProtectionProvider = options.DataProtectionProvider;

                if (dataProtectionProvider != null)
                {
                    appUserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("SECURITY.API"))
                    {
                        //Código para la el mail de confirmación y tiempo de vida de reset de password
                        TokenLifespan = TimeSpan.FromMinutes(20)
                    };
                }

                return appUserManager;
            }

        }

        /// <summary>
        /// Gestiona los inicios de sesión
        /// </summary>
        public class SignInManager : SignInManager<User, string>
        {
            public SignInManager(UserManager userManager, IAuthenticationManager authenticationManager)
                : base(userManager, authenticationManager)
            {
            }

            public static SignInManager Create(IdentityFactoryOptions<SignInManager> options, IOwinContext context)
            {
                return new SignInManager(context.GetUserManager<UserManager>(), context.Authentication);
            }
        }

        /// <summary>
        /// Genera el token de seguridad
        /// </summary>
        public class AccessTokenFormat : ISecureDataFormat<AuthenticationTicket>
        {
            /// <summary>
            /// Transforms the specified authentication ticket into a JWT.
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public string Protect(AuthenticationTicket data)
            {
                if (data == null)
                {
                    throw new ArgumentNullException("data");
                }

                string issuer = ConfigurationManager.AppSettings["issuer"];
                string audienceId = data.Properties.Dictionary["AudienceId"]; //ConfigurationManager.AppSettings["as:AudienceId"];
                string audienceSecret = data.Properties.Dictionary["AudienceSecret"]; //ConfigurationManager.AppSettings["as:AudienceSecret"];

                var symmetricKey = Convert.FromBase64String(audienceSecret);
                var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature);

                var issued = data.Properties.IssuedUtc;
                var expires = data.Properties.ExpiresUtc;

                var token = new JwtSecurityToken(issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingCredentials);
                var handler = new JwtSecurityTokenHandler();

                var jwt = handler.WriteToken(token);

                return jwt;
            }

            /// <summary>
            /// Validates the specified JWT and builds an AuthenticationTicket from it.
            /// </summary>
            /// <param name="protectedText"></param>
            /// <returns></returns>
            public AuthenticationTicket Unprotect(string protectedText)
            {
                throw new NotImplementedException();
            }
        }
    }

    namespace Identity.Providers
    {
        public class OAuthProvider : IOAuthAuthorizationServerProvider
        {

            public System.Threading.Tasks.Task AuthorizationEndpointResponse(OAuthAuthorizationEndpointResponseContext context)
            {
                throw new NotImplementedException();
            }

            public System.Threading.Tasks.Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
            {
                throw new NotImplementedException();
            }

            public System.Threading.Tasks.Task GrantAuthorizationCode(OAuthGrantAuthorizationCodeContext context)
            {
                throw new NotImplementedException();
            }

            public System.Threading.Tasks.Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
            {
                throw new NotImplementedException();
            }

            public System.Threading.Tasks.Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
            {
                throw new NotImplementedException();
            }

            public async System.Threading.Tasks.Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
            {

                var originalClient = context.Ticket.Properties.Dictionary["ClientId"];
                //var currentClient = context.OwinContext.Get<string>("ClientId");

                var userManager = context.OwinContext.GetUserManager<UserManager>();
                User user = await userManager.FindByNameAsync(context.Ticket.Identity.GetUserName());

                // enforce client binding of refresh token
                if (user == null || originalClient != user.InternalId)
                {
                    context.Rejected();
                    return;
                }

                // chance to change authentication ticket for refresh token requests
                var newId = new ClaimsIdentity(context.Ticket.Identity);

                newId.AddClaim(new Claim("newClaim", "refreshToken"));

                var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);

                context.Validated(newTicket);
            }

            /// <summary>
            /// Called when a request to the Token endpoint arrives with a "grant_type" of "password". 
            /// This occurs when the user has provided name and password credentials directly into the client application's user interface,
            /// and the client application is using those to acquire an "access_token" and optional "refresh_token". If the web application 
            /// supports the resource owner credentials grant type it must validate the context.Username and context.Password as appropriate. 
            /// To issue an access token the context.Validated must be called with a new ticket containing the claims about the resource owner 
            /// which should be associated with the access token. The application should take appropriate measures to ensure that the endpoint isn’t 
            /// abused by malicious callers. The default behavior is to reject this grant type. See also http://tools.ietf.org/html/rfc6749#section-4.3.2
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {
                var allowedOrigin = "*";

                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

                var userManager = context.OwinContext.GetUserManager<UserManager>();

                //var signInManager = context.OwinContext.Get<SignInManager>();

                User user = await userManager.FindByNameAsync(context.UserName);

                if (user != null)
                {
                    if (userManager.SupportsUserLockout && userManager.IsLockedOut(user.Id))
                    {

                        context.SetError(RESOURCES.Core.cuentaBloqueada, string.Format(RESOURCES.Core.cuentaBloqueadaDescripcion, Utilities.DateTime.ToLocalTime(user.LockoutEndDateUtc.ToString()).ToString()));
                        return;
                    }

                    user = await userManager.FindByIdAsync(user.Id);

                    if (userManager.CheckPassword(user, context.Password))
                    {
                        if (userManager.SupportsUserLockout && userManager.GetAccessFailedCount(user.Id) > 0)
                        {
                            userManager.ResetAccessFailedCount(user.Id);
                        }

                        //Autentica 
                        user = await userManager.FindAsync(context.UserName, context.Password);

                        if (user != null)
                        {
                            var entities = new securityEntities();
                            var audienceId = context.Request.Query["AudienceId"];

                            var audience = entities.audiences.Where(x => x.InternalId == audienceId).FirstOrDefault();

                            if (audience != null && audience.IsActive)
                            {
                                if (audience.ExpirationDate == null || audience.ExpirationDate >= DateTime.Now)
                                {
                                    ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(user, "JWT");
                                    AuthenticationProperties oAuthProperties = new AuthenticationProperties();

                                    oAuthProperties.Dictionary.Add("ClientId", user.InternalId);
                                    oAuthProperties.Dictionary.Add("AudienceId", audienceId);
                                    oAuthProperties.Dictionary.Add("AudienceSecret", audience.Secret);

                                    var ticket = new AuthenticationTicket(oAuthIdentity, oAuthProperties);

                                    context.Validated(ticket);
                                }
                                else
                                {
                                    context.SetError(RESOURCES.Core.invalidGrant, RESOURCES.Core.invalidGrantDescription);
                                }
                            }
                            else
                            {
                                context.SetError(RESOURCES.Core.invalidGrant, RESOURCES.Core.invalidGrantDescription);
                            }
                        }

                    }
                    else
                    {
                        if (userManager.SupportsUserLockout && userManager.GetLockoutEnabled(user.Id))
                        {
                            userManager.AccessFailed(user.Id);
                            context.SetError(RESOURCES.Core.invalidGrant, RESOURCES.Core.invalidGrantDescription);
                            return;
                        }
                    }
                }
                else
                {
                    context.SetError(RESOURCES.Core.invalidGrant, RESOURCES.Core.invalidGrantDescription);
                    return;
                }
            }

            public System.Threading.Tasks.Task MatchEndpoint(OAuthMatchEndpointContext context)
            {
                //if (context.OwinContext.Request.Method == "OPTIONS" && context.IsTokenEndpoint)
                //{
                //    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "POST" });
                //    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "accept", "authorization", "content-type" });
                //    context.OwinContext.Response.StatusCode = 200;
                //    context.RequestCompleted();
                //    return Task.FromResult<object>(null);
                //}
                return Task.FromResult<object>(context);
            }

            public System.Threading.Tasks.Task TokenEndpoint(OAuthTokenEndpointContext context)
            {
                foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
                {
                    context.AdditionalResponseParameters.Add(property.Key, property.Value);
                }

                return Task.FromResult<object>(null);
            }

            /// <summary>
            /// Called before the TokenEndpoint redirects its response to the caller
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public System.Threading.Tasks.Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
            {
                //TODO: Aquí tengo el token después de redireccionar al caller,
                //lo puedo usar para almacenarlo en bbdd para realizar revocaciones ...
                var accessToken = context.AccessToken;
                return Task.FromResult<object>(null);
            }

            public System.Threading.Tasks.Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
            {
                throw new NotImplementedException();
            }

            public async System.Threading.Tasks.Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                context.Validated();
                return;
            }

            public System.Threading.Tasks.Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
            {
                throw new NotImplementedException();
            }

            public System.Threading.Tasks.Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
            {
                context.Validated();
                return Task.FromResult<object>(null);
            }
        }

        public class RefreshTokenProvider : IAuthenticationTokenProvider
        {
            private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

            public async Task CreateAsync(AuthenticationTokenCreateContext context)
            {
                //USE of refresh token: http://yourdomain/Token?username=yourusername&refresh_token=969c9b04-afe5-48a3-9353-62509f71e906&grant_type=refresh_token

                var userManager = context.OwinContext.GetUserManager<UserManager>();
                User user = await userManager.FindByNameAsync(context.Ticket.Identity.GetUserName());

                var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary);

                if (user.RefreshTokenExpiresUtc == null || Utilities.DateTime.ToLocalTime(user.RefreshTokenExpiresUtc.ToString()) <= DateTime.Now)
                {
                    user.RefreshToken = Guid.NewGuid().ToString();

                    refreshTokenProperties.IssuedUtc = context.Ticket.Properties.IssuedUtc;
                    refreshTokenProperties.ExpiresUtc = DateTime.UtcNow.AddMonths(6);

                    user.RefreshTokenIssuedUtc = refreshTokenProperties.IssuedUtc.Value.UtcDateTime;
                    user.RefreshTokenExpiresUtc = refreshTokenProperties.ExpiresUtc.Value.UtcDateTime;

                    await userManager.UpdateAsync(user);

                }

                refreshTokenProperties.IssuedUtc = user.RefreshTokenIssuedUtc;
                refreshTokenProperties.ExpiresUtc = user.RefreshTokenExpiresUtc;

                var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

                _refreshTokens.TryAdd(user.RefreshToken, refreshTokenTicket);

                context.SetToken(user.RefreshToken);
            }

            public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
            {
                //Aquí es donde creamos el refresh
                AuthenticationTicket ticket;
                if (_refreshTokens.TryRemove(context.Token, out ticket))
                {
                    context.SetTicket(ticket);
                }
            }

            public void Create(AuthenticationTokenCreateContext context)
            {
                throw new NotImplementedException();
            }

            public void Receive(AuthenticationTokenReceiveContext context)
            {
                throw new NotImplementedException();
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
        /// Genera el token de seguridad
        /// </summary>
        public static class Token
        {
            public static string Generate(AuthenticationTicket ticket)
            {
                var entities = new securityEntities();
                var audienceId = ticket.Properties.Dictionary["AudienceId"];

                var audience = entities.audiences.Where(x => x.InternalId == audienceId).FirstOrDefault();

                if (audience != null && audience.IsActive)
                {
                    if (audience.ExpirationDate == null || audience.ExpirationDate >= DateTime.Now)
                    {
                        string issuer = ConfigurationManager.AppSettings["issuer"];

                        var symmetricKey = Convert.FromBase64String(audience.Secret);
                        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature);

                        var issued = ticket.Properties.IssuedUtc;
                        var expires = ticket.Properties.ExpiresUtc;

                        var token = new JwtSecurityToken(issuer, audienceId, ticket.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingCredentials);
                        var handler = new JwtSecurityTokenHandler();

                        var jwt = handler.WriteToken(token);

                        SwaggerConfig.AccessToken = string.Format("bearer {0}", jwt);

                        return jwt;
                    }
                }

                return null;
            }
        }

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

                    if (jwtToken.Claims != null)
                    {
                        var entities = new securityEntities();

                        var claim = jwtToken.Claims.Where(x => x.Type == "aud").FirstOrDefault();
                        var audienceId = claim?.Value;
                        
                        if (audienceId!=null)
                        {
                            var audience = entities.audiences.Where(x => x.InternalId == audienceId).FirstOrDefault();

                            if (audience!=null && audience.ExpirationDate == null || audience.ExpirationDate >= DateTime.Now)
                            {
                                var symmetricKey = Convert.FromBase64String(audience.Secret);

                                var validationParameters = new TokenValidationParameters()
                                {
                                    RequireExpirationTime = true,
                                    ValidIssuer = ConfigurationManager.AppSettings["issuer"],
                                    ValidAudience = audienceId,
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                                };

                                Principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
                            }
                        }

                    }

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
                catch(Exception e)
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