using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SECURITY.MODEL.Entities
{
    public class Audience
    {
        [Key]
        [MaxLength(36)]
        public string AudienceId { get; set; }

        [MaxLength(36)]
        public string InternalId { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        [MaxLength(128)]
        public string Secret { get; set; }

        public Int32 DaysToExpire { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public bool IsInternal { get; set; }
        public bool IsActive { get; set; }

        public ICollection<User> Users {get;set;}
    }
}
