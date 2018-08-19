using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Docs.Models;
using Docs.Data;
using Microsoft.AspNetCore.Authorization;

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
            string userId = GetUserId(User.Identity.Name);
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
                doc.UserId = GetUserId(User.Identity.Name);
                doc.Content = string.Empty;
                db.Documents.Add(doc);
                db.SaveChanges();
                return RedirectToAction("Document", new { Id = doc.Id });
            }
            return View();
        }

        private string GetUserId(string name) =>
            db.Users.First(u => name == u.UserName).Id;
        private Document GetDocumentById(int id) =>
            db.Documents.First(d => d.Id == id);

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
