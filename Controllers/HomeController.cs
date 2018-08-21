using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Docs.Models;
using Docs.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Docs.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DocsDbContext db;

        public HomeController(DocsDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            string userId = GetUserIdByName(User.Identity.Name);
            return View(db.Documents.Where(d => d.UserId == userId));
        }

        public IActionResult Document(int id)
        {
            return View(GetDocumentById(id));
        }

        [HttpPost]
        public void DocumentChange(Document doc)
        {
            GetDocumentById(doc.Id).Content = doc.Content;
            db.SaveChanges();
        }

        [HttpGet]
        public IActionResult DeleteDocument(int id)
        {
            return View(GetDocumentById(id));
        }
        [HttpPost]
        public IActionResult DeleteDocument(string name, int id)
        {
            Document doc = GetDocumentById(id);
            if (name == doc.Name)
            {
                db.Documents.Remove(doc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doc);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Document doc)
        {
            if (ModelState["Name"].Errors.Count == 0)
            {
                doc.UserId = GetUserIdByName(User.Identity.Name);
                doc.Content = string.Empty;
                db.Documents.Add(doc);
                db.SaveChanges();
                return RedirectToAction("Document", new { Id = doc.Id });
            }
            return View();
        }

        [HttpGet]
        public IActionResult Members(int id)
        {
            var t = new Tuple<int, IEnumerable<DocumentMember>, IEnumerable<Role>>(id, 
                db.DocumentMembers.Where(m => m.DocumentId == id).Include(m => m.User).Include(m => m.Role),
                db.MembersRoles);
            return View(t);
        }
        [HttpPost]
        public DocumentMember AddMember(DocumentMember m, string userName)
        {
            m.UserId = GetUserIdByName(userName);
            if (m.UserId != null && 
                db.DocumentMembers.FirstOrDefault(dm => dm.UserId == m.UserId && dm.DocumentId == m.DocumentId) == null)
            {
                db.DocumentMembers.Add(m);
                db.SaveChanges();

                m.User = db.Users.First(u => u.Id == m.UserId);
                m.Role = db.MembersRoles.First(r => r.Id == m.RoleId);
                return m;
            }
            return null;
        }
        [HttpPost]
        public void DeleteMember(int documentId, string userName)
        {
            string userId = GetUserIdByName(userName);
            db.DocumentMembers.Remove(db.DocumentMembers.First(m => m.DocumentId == documentId && m.UserId == userId));
            db.SaveChanges();
        }

        private string GetUserIdByName(string name) =>
            db.Users.FirstOrDefault(u => name == u.UserName)?.Id;

        private Document GetDocumentById(int id) =>
            db.Documents.FirstOrDefault(d => d.Id == id);

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
