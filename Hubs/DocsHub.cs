using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docs.Hubs.DocumentHubMapper;
using Docs.Services.Interfaces;

namespace Docs.Hubs
{
    [Authorize]
    public class DocsHub : Hub
    {
        private static readonly DocumentMapper mapper = new DocumentMapper();
        private readonly IDocuments documents;

        public DocsHub(IDocuments documents)
        {
            this.documents = documents;
        }

        public async Task TextChange(int docId, object docInput)
        {
            await Clients.Users(mapper.GetUsers(docId)
                .Select(u => u.Id)
                .Except(new string[] { Context.UserIdentifier })
                .ToArray())
                .SendAsync("TextChange", docInput);
        }

        public async Task JoinDocument(int docId)
        {
            mapper.AddUser(docId, new DocumentMapperUser() { Name = Context.User.Identity.Name, Id = Context.UserIdentifier });
            await SendChangeOnlineUsers(docId);
        }
        public async Task LeaveDocument(int docId)
        {
            mapper.RemoveUser(docId, Context.User.Identity.Name);
            await SendChangeOnlineUsers(docId);
        }
        public static void LeaveDocumentStatic(int docId, string userName)
        {
            mapper.RemoveUser(docId, userName);
        }
        public async Task NameChange(int docId, string name)
        {
            await Clients.Users(mapper.GetUsers(docId).Select(u => u.Id)
                .Except(new string[] { Context.UserIdentifier })
                .ToList()).SendAsync("NameChange", name);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            base.OnDisconnectedAsync(exception);
            int docId = mapper.FindDocumentIdByUserName(Context.User.Identity.Name);
            return LeaveDocument(docId);
        }

        private async Task SendChangeOnlineUsers(int docId)
        {
            var users = mapper.GetUsers(docId);
            await Clients.Users(users.Select(u => u.Id).ToList())
            .SendAsync("ChangeOnlineUsers", users.Select(u => u.Name));
        }

        private bool CheckUser(int docId, string userName)
        {
            return mapper.GetUsers(docId).FirstOrDefault(u => u.Name == userName) != null;
        }
    }
}
