using Docs.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Docs.Controllers
{
    public class DocumentHelper
    {
        public int DocumentId { get; set; }
        public IdentityUser ChangingUser { get; set; }
        public List<IdentityUser> OnlineUsers { get; private set; }

        public DocumentHelper(int id)
        {
            DocumentId = id;
        }
    }
}
