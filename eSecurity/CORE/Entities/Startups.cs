using Microsoft.AspNet.Identity;
using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using SECURITY.MODEL.Entities;
using SECURITY.MODEL.Interfaces;
using System.Configuration;
using System.Data.Entity;

namespace SECURITY.CORE.Entities
{
    public class Startups : IStartups
    {
        public DbContext DbContext { get; set; }
        public RoleManager<Role> RoleStore { get; set; }
        public UserManager<User> UserStore { get; set; }

        public IPrincipal User { get; set; }

        public async Task<bool> Init()
        {
            try
            {
                var filePath = ConfigurationManager.AppSettings["startup:scripts:path"];

                var scriptsFile = string.Format("{0}{1}", filePath, "scripts.txt");

                if (scriptsFile != null)
                {
                    StreamReader scriptsReader = new StreamReader(scriptsFile);
                    StringBuilder scripts = new StringBuilder(scriptsReader.ReadToEnd());

                    DbContext.Database.ExecuteSqlCommand(scripts.ToString());
                }

                var audiencesFile = string.Format("{0}{1}", filePath, "audiences.txt");

                if (audiencesFile != null)
                {
                    var Audiences = new Audiences();
                    Audiences.Entities = new MODEL.securityEntities();

                    StreamReader audiencesReader = new StreamReader(audiencesFile);
                    StringBuilder audiences = new StringBuilder(audiencesReader.ReadToEnd());

                    foreach (var entry in audiences.ToString().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (entry != string.Empty && entry.Substring(0, 2) != "//")
                        {
                            var interno = entry.Contains('*');

                            var audience = new Audience
                            {
                                AudienceId = Guid.NewGuid().ToString(),
                                Name = entry.Split(',')[0],
                                InternalId = entry.Split(',')[1],
                                Secret = entry.Split(',')[2],
                                DaysToExpire = Int32.Parse(entry.Split(',')[3]),
                                IsInternal = interno,
                                IsActive = true,
                                CreationDate = DateTime.Now
                            };

                            await Audiences.Post(audience);
                        }
                    }
                }

                var rolesFile = string.Format("{0}{1}", filePath, "roles.txt");

                if (rolesFile != null)
                {
                    StreamReader rolesReader = new StreamReader(rolesFile);
                    StringBuilder roles = new StringBuilder(rolesReader.ReadToEnd());

                    foreach (var entry in roles.ToString().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (entry != string.Empty && entry.Substring(0, 2) != "//")
                        {
                            var interno = entry.Contains('*');

                            RoleStore.Create(new Role
                            {
                                InternalId = Guid.NewGuid().ToString(),
                                Name = entry.Replace("*", ""),
                                CreationDate = DateTime.Now,
                                IsInternal = interno,
                                IsActive = true
                            });
                        }
                    }

                    var usersFile = string.Format("{0}{1}", filePath, "usuarios.txt");

                    if (usersFile != null)
                    {

                        var sec = new MODEL.securityEntities();

                        StreamReader usersReader = new StreamReader(usersFile);
                        StringBuilder users = new StringBuilder(usersReader.ReadToEnd());

                        foreach (var entry in users.ToString().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (entry != string.Empty && entry.Substring(0, 2) != "//")
                            {

                                var audienceId = entry.Split('#')[0];
                                var accountInfo = entry.Split('#')[1];
                                var userInfo = entry.Split('#')[2];
                                var rolesInfo = entry.Split('#')[3];

                                var userName = userInfo.Split('|')[0];
                                var password = userInfo.Split('|')[1];

                                var interno = userName.Contains('*');
                                var aud = sec.audiences.Where(x => x.InternalId == audienceId).FirstOrDefault();  

                                var user = new User
                                {
                                    InternalId = Guid.NewGuid().ToString(),
                                    AudienceId = aud.AudienceId,
                                    FirstName = accountInfo.Split(',')[0],
                                    LastName = accountInfo.Split(',')[1],
                                    Email = accountInfo.Split(',')[2],
                                    UserName = userName.Replace("*", ""),
                                    IsInternal = interno,
                                    IsActive = true,
                                    JoinDate = DateTime.Now,
                                    CreationDate = DateTime.Now
                                };

                                var ir = UserStore.Create(user, password);

                                if (ir.Succeeded)
                                {
                                    UserStore.AddToRoles(user.Id, rolesInfo.Split(','));
                                }
                            }

                        }
                    }

                    return true;
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
