using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docs.Hubs.DocumentHubMapper;

namespace Docs.Hubs
{
    [Authorize]
    public class DocsHub : Hub
    {
        private static readonly DocumentMapper mapper = new DocumentMapper();

        public async Task TextChange(int docId, object docInput)
        {
            await Clients.Users(mapper.GetUsers(docId).Select(u => u.Id)
                .Except(new string[] { Context.UserIdentifier })
                .ToList()).SendAsync("TextChange", docInput);
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

        private async Task SendChangeOnlineUsers(int docId) =>
            await Clients.Users(mapper.GetUsers(docId).Select(u => u.Id).ToList())
            .SendAsync("ChangeOnlineUsers", mapper.GetUsers(docId).Select(u => u.Name));
    }
}
