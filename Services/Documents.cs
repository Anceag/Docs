using Docs.Controllers;
using Docs.Data;
using Docs.Models;
using Docs.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Docs.Services
{
    public class Documents : IDocuments
    {
        private readonly DocsDbContext db;

        public Documents(DocsDbContext db)
        {
            this.db = db;
        }

        public IEnumerable<Role> Roles { get => db.MembersRoles.AsEnumerable(); }

        public IQueryable<Document> GetDocuments(string userId)
        {
            return db.Documents.Where(d => d.UserId == userId);
        }

        public Document GetDocument(int docId)
        {
            return db.Documents
                .Include(d => d.User)
                .FirstOrDefault(d => d.Id == docId);
        }

        public Document AddDocument(string docName, string userId)
        {
            var doc = new Document
            {
                UserId = userId,
                Name = docName,
                Content = string.Empty
            };
            db.Documents.Add(doc);
            db.SaveChanges();
            return doc;
        }

        public void SetDocumentContent(int docId, string content)
        {
            Document doc = GetDocument(docId);
            doc.Content = content ?? string.Empty;
            db.SaveChanges();
        }

        public bool DeleteDocument(string docName, int docId)
        {
            Document doc = GetDocument(docId);
            if (docName == doc.Name)
            {
                db.Documents.Remove(doc);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        
        public DocumentMember GetMember(string userId, int docId)
        {
            return db.DocumentMembers
                .Include(m => m.User)
                .Include(m => m.Document)
                .Include(m => m.Role)
                .FirstOrDefault(m => m.UserId == userId && m.DocumentId == docId);
        }

        public DocumentMember AddMember(int docId, string userId, int roleId)
        {
            if (userId != null && GetMember(userId, docId) == null && GetDocument(docId).UserId != userId)
            {
                var member = new DocumentMember
                {
                    DocumentId = docId,
                    UserId = userId,
                    RoleId = roleId
                };
                db.DocumentMembers.Add(member);
                db.SaveChanges();

                member.User = db.Users.First(u => u.Id == member.UserId);
                member.Role = db.MembersRoles.First(r => r.Id == member.RoleId);
                return member;
            }
            return null;
        }

        public void DeleteMember(string userId, int docId)
        {
            db.DocumentMembers.Remove(GetMember(userId, docId));
            db.SaveChanges();
        }

        public IQueryable<DocumentMember> GetDocumentMembers(int docId)
        {
            return db.DocumentMembers.Where(m => m.DocumentId == docId)
                .Include(m => m.User)
                .Include(m => m.Role);
        }

        public IQueryable<DocumentMember> GetUserMembers(string userId)
        {
            return db.DocumentMembers.Where(m => m.UserId == userId)
                .Include(m => m.Document)
                .Include(m => m.Role);
        }

        public Role GetRole(int roleId)
        {
            return db.MembersRoles.First(r => r.Id == roleId);
        }

        public MemoryStream GetDocumentMemoryStream(int id)
        {
            Document doc = GetDocument(id);
            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);
            tw.Write(doc.Content.Replace("\n", "\r\n"));
            tw.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}
