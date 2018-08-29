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
using System.IO;
using Microsoft.AspNetCore.SignalR;
using Docs.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Docs.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDocuments documents;
        private readonly UserManager<IdentityUser> userManager;

        public HomeController(IDocuments documents, UserManager<IdentityUser> userManager)
        {
            this.documents = documents;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            string userId = userManager.GetUserId(User);
            return View(documents.GetDocuments(userId));
        }

        public IActionResult Document(int id)
        {
            string userId = userManager.GetUserId(User);
            var doc = documents.GetDocument(id);
            DocumentMember member = null;

            if (doc.UserId != userId)
            {
                member = documents.GetMember(userId, id);
                if (member == null)
                    return NotFound();
                member.Role = documents.GetRole(member.RoleId);
            }

            var t = new Tuple<Document, DocumentMember>(doc, member);
            return View(t);
        }

        public object GetDocumentInfo(int id)
        {
            return new { documents.GetDocument(id).Content, changingUser = documents.GetDocumentHelper(id)?.ChangingUser?.UserName };
        }

        public FileResult DownloadDocument(int id)
        {
            var ms = documents.GetDocumentMemoryStream(id);
            return File(ms, "text/plain", id + ".txt");
        }

        [HttpPost]
        public async void DocumentStartChange(int id)
        {
            var docHelper = documents.GetDocumentHelper(id);
            docHelper.ChangingUser = await userManager.GetUserAsync(User);
        }
        [HttpPost]
        public void DocumentChange(int id, string content)
        {
            documents.SetDocumentContent(id, content);
        }

        [HttpGet]
        public IActionResult DeleteDocument(int id)
        {
            return View(documents.GetDocument(id));
        }
        [HttpPost]
        public IActionResult DeleteDocument(string name, int id)
        {
            if (documents.DeleteDocument(name, id))
                return RedirectToAction("Index");
            return View(id);
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
                int id = documents.AddDocument(doc.Name, userManager.GetUserId(User)).Id;
                return RedirectToAction("Document", new { Id = id });
            }
            return View();
        }

        [HttpGet]
        public IActionResult Members(int id)
        {
            var t = new Tuple<int, IEnumerable<DocumentMember>, IEnumerable<Role>>(id,
                documents.GetDocumentMembers(id),
                documents.Roles);
            return View(t);
        }
        [HttpPost]
        public DocumentMember AddMember(DocumentMember m, string userName)
        {
            return documents.AddMember(m.DocumentId, userManager.GetUserId(User), m.RoleId);
        }
        [HttpPost]
        public async void DeleteMember(int documentId, string userName)
        {
            documents.DeleteMember((await userManager.FindByNameAsync(userName)).Id, documentId);
        }

        public IActionResult OtherDocuments()
        {
            return View(documents.GetUserMembers(userManager.GetUserId(User)));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
