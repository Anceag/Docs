using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Docs.Models
{
    public class DocumentMember
    {
        [Key]
        public int DocumentId { get; set; }
        [Key]
        public string UserId { get; set; }
        [Required]
        public int RoleId { get; set; }

        public Document Document { get; set; }
        public IdentityUser User { get; set; }
        public Role Role { get; set; }
    }
}
