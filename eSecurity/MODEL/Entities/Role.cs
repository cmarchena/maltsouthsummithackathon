using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SECURITY.MODEL.Entities
{
    public class Role : IdentityRole 
    {
        [Required]
        [MaxLength(36)]
        public string InternalId { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public bool IsInternal { get; set; }
        public bool IsActive { get; set; }
    }
}
