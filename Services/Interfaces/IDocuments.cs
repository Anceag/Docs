using Docs.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Docs.Services.Interfaces
{
    public interface IDocuments
    {
        IEnumerable<Role> Roles { get; }

        IQueryable<Document> GetDocuments(string userId);
        Document GetDocument(int docId);
        Document AddDocument(string docName, string userId);
        void SetDocumentContent(int docId, string content);
        bool DeleteDocument(string docName, int docId);
        Controllers.DocumentHelper GetDocumentHelper(int docId);
        DocumentMember GetMember(string userId, int docId);
        DocumentMember AddMember(int docId, string userId, int roleId);
        void DeleteMember(string userId, int docId);
        IQueryable<DocumentMember> GetDocumentMembers(int docId);
        IQueryable<DocumentMember> GetUserMembers(string userId);
        Role GetRole(int roleId);
        MemoryStream GetDocumentMemoryStream(int id);
    }
}
