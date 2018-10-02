using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SECURITY.CORE.Globals;
using SECURITY.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECURITY.CORE.Entities.KK
{
    public class UserManager<TUser> : UserStore<TUser>
  where TUser : User
    {
        public UserManager(DbContext context)
          : base(context)
        {
        }

        public string AudienceId { get; set; }

        public override Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AudienceId = AudienceId;
            return base.CreateAsync(user);
        }

        public override Task<TUser> FindByEmailAsync(string email)
        {
            return this.GetUserAggregateAsync(u => u.Email.ToUpper() == email.ToUpper()
                && u.AudienceId == AudienceId);
        }

        public override Task<TUser> FindByNameAsync(string userName)
        {
            return this.GetUserAggregateAsync(u => u.UserName.ToUpper() == userName.ToUpper()
                && u.AudienceId == AudienceId);
        }
    }

    //public class UserManager<TUser> : UserManager<User, string> where TUser : class, IUser<string>
    //{
    //    public UserManager(IUserStore<User, string> store) : base(store)
    //    {
    //    }

    //    public string AudienceId { get; set; }

    //    public override Task<IdentityResult> CreateAsync(User user, string password)
    //    {
    //        if (user == null)
    //        {
    //            throw new ArgumentNullException("user");
    //        }

    //        //user.AudienceId = AudienceId;
    //        return base.CreateAsync(user, password);
    //    }

    //    public override Task<User> FindByEmailAsync(string email)
    //    {
    //        return Users.Where(u => u.Email.ToUpper() == email.ToUpper() && u.AudienceId == AudienceId).FirstOrDefault();
    //    }
    //}
}
