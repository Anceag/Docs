using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Docs.Models.ViewModels
{
    public class MembersViewModel
    {
        public int DocumentId { get; set; }
        public IEnumerable<DocumentMember> Members { get; set; }
        public IEnumerable<Role> Roles { get; set; }
    }
}
