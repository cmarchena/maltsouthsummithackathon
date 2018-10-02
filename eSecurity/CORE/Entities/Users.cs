using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

using SECURITY.MODEL.Entities;
using SECURITY.MODEL.Interfaces;

namespace SECURITY.CORE.Entities
{
    public class Users : IUsers
    {

        public UserManager<User> UserStore { get; set; }

        public IPrincipal User { get; set; }

        public async Task<User> Get(Guid internalId)
        {
            try
            {
                var user = UserStore.Users.Single(x => x.InternalId == internalId.ToString());
                return await UserStore.FindByIdAsync(user.Id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public async Task<IdentityResult> Post(User model, string password)
        {

            try
            {
                model.InternalId = model.InternalId;
                model.CreationDate = DateTime.Now;
                return await UserStore.CreateAsync(model, password);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public async Task<IdentityResult> Put(User model)
        {
            try
            {
                var user = UserStore.Users.Single(x => x.InternalId == model.InternalId);

                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;
                user.ModificationDate = model.ModificationDate;
                user.IsActive = model.IsActive;

                return await UserStore.UpdateAsync(user);
                //return await UserStore.UpdateAsync(model);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public async Task<IdentityResult> Delete(Guid internalId)
        {
            try
            {
                var user = UserStore.Users.Single(x => x.InternalId == internalId.ToString());
                return await UserStore.DeleteAsync(user);   
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
   
        }

        public async Task<IdentityResult> ChangePassword(Guid internalId, string currentPassword, string newPassword)
        {
            try
            {
                User user = await Get(internalId);
                return await UserStore.ChangePasswordAsync(user.Id, currentPassword, newPassword);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        private System.DateTime? ToLocalTime(string date)
        {
            if (date != string.Empty && date != null)
            {
                TimeZone zone = TimeZone.CurrentTimeZone;
                System.DateTime local = zone.ToLocalTime(System.DateTime.Parse(date));
                return local;
            }
            return null;
        }

        public async Task<bool> CheckPassword(string userName, string password)
        {
            try
            {
                var user = await UserStore.FindByNameAsync(userName);
                var r = false;

                if (user != null)
                {
                    if (UserStore.SupportsUserLockout && !UserStore.IsLockedOut(user.Id))
                    {
                        r = await UserStore.CheckPasswordAsync(user, password);

                        if (!r)
                        {
                            var ir = await UserStore.AccessFailedAsync(user.Id);

                            if (ToLocalTime(user.LockoutEndDateUtc.ToString()) > DateTime.Now)
                            {
                                await UserStore.SetLockoutEnabledAsync(user.Id, true);
                            }
                        }
                        else
                        {
                            await UserStore.ResetAccessFailedCountAsync(user.Id);
                            await UserStore.SetLockoutEnabledAsync(user.Id, false);
                        }
                    }
                }

                return r;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        /// <summary>
        /// Devuelve true si el usuario contiene el rol
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="role">Rol1</param>
        /// <returns></returns>
        public async Task<bool> IsInRole(string userName, string role)
        {
            try
            {
                var user = await UserStore.FindByNameAsync(userName);

                if (user != null)
                {
                    return await UserStore.IsInRoleAsync(user.Id, role);
                }

                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        /// <summary>
        /// Devuelve los roles del usuario
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <returns></returns>
        public async Task<IList<string>> GetRoles(string userName)
        {
            try
            {
                var user = UserStore.Users.Single(x => x.UserName == userName);

                if (user != null)
                {
                    return await UserStore.GetRolesAsync(user.Id);
                }

                return null;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        /// <summary>
        /// Añade al usuario al rol
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="role">Rol1</param>
        /// <returns></returns>
        public async Task<User> AddToRole(string userName, string role)
        {
            try
            {
                var user = await UserStore.FindByNameAsync(userName);

                if (user != null)
                {
                    var ir = await UserStore.AddToRoleAsync(user.Id, role);
                }

                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        /// <summary>
        /// Añade al usuario a la lista de roles separados por comas
        /// </summary>
        /// <param name="userName">NombreUsuario</param>
        /// <param name="roles">Rol1,Rol2,Rol3</param>
        /// <returns></returns>
        public async Task<User> AddToRoles(string userName, string roles)
        {
            try
            {
                var user = await UserStore.FindByNameAsync(userName);

                if (user != null)
                {
                    var ir = await UserStore.AddToRolesAsync(user.Id, roles.Split(','));
                }

                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public async Task<User> RemoveFromRole(string userName, string role)
        {
            try
            {
                var user = await UserStore.FindByNameAsync(userName);

                if (user != null)
                {
                    var ir = await UserStore.RemoveFromRoleAsync(user.Id, role);
                }

                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public async Task<User> RemoveFromRoles(string userName, string roles)
        {
            try
            {
                var user = await UserStore.FindByNameAsync(userName);

                if (user != null)
                {
                    var ir = await UserStore.RemoveFromRolesAsync(user.Id, roles.Split(','));
                }

                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }
    }
}
