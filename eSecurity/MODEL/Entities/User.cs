using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SECURITY.MODEL.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(36)]
        public string InternalId { get; set; }

        [Index("UserNameIndex", 2, IsUnique = true)]
        [MaxLength(36)]
        public string AudienceId { get; set; }

        [ForeignKey("AudienceId")]
        public Audience Audience { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public string PasswordResetToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenIssuedUtc { get; set; }

        public DateTime RefreshTokenExpiresUtc { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public bool IsInternal { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public DateTime JoinDate { get; set; }

    }
}
