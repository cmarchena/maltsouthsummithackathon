using Microsoft.AspNet.Identity.EntityFramework;

namespace SECURITY.MODEL.Entities
{
    class UserClaim : IdentityUserClaim
    {
        public string Type { get; private set; }
        public string Value { get; private set; }
    }
}
