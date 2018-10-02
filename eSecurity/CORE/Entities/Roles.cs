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
    public class Roles : IRoles
    {

        public RoleManager<Role> RoleStore { get; set; }
        public UserManager<User> UserStore { get; set; }

        public IPrincipal User { get; set; }

        public async Task<Role> Get(Guid internalId)
        {
            try
            {
                var role = RoleStore.Roles.Single(x => x.InternalId == internalId.ToString());
                return await RoleStore.FindByIdAsync(role.Id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public async Task<Role> Get(string name)
        {
            try
            {
                return await RoleStore.FindByNameAsync(name);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public async Task<IdentityResult> Post(Role model)
        {

            try
            {
                model.InternalId = Guid.NewGuid().ToString();
                return await RoleStore.CreateAsync(model);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public async Task<IdentityResult> Put(Role model)
        {
            try
            {
                return await RoleStore.UpdateAsync(model);
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
                var role = RoleStore.Roles.Single(x => x.InternalId == internalId.ToString());
                return await RoleStore.DeleteAsync(role);   
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
   
        }

        public async Task<IList<string>> GetUsersInRole(string name)
        {
            try
            {
                var usersRoles = from role in RoleStore.Roles
                                      where role.Name == name
                                      from user in role.Users
                                      select user;

                var users = new List<string>();

                foreach (var userRole in usersRoles)
                {
                    var user = UserStore.FindById(userRole.UserId);
                    users.Add(user.InternalId);
                }

                return users;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public async Task<bool> IsInRole(string userName, string roleName)
        {
            try
            {
                var u = (from user in UserStore.Users
                        where user.UserName == userName
                        select user).FirstOrDefault();

                if (u != null)
                {

                    var r = (from role in RoleStore.Roles
                            where role.Name == roleName
                            from user in role.Users
                            where user.UserId == u.Id 
                            select user).FirstOrDefault();

                    if (r != null) return true;
                }

                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
