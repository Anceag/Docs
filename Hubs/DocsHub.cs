using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Docs.Hubs
{
    [Authorize]
    public class DocsHub : Hub
    {
        private static readonly DocumentMapper mapper = new DocumentMapper();

        public async Task TextChange(string message)
        {
            await Clients.Others.SendAsync("TextChange", message);
        }

        public async Task JoinDocument(int docId)
        {
            mapper.AddUser(docId, Context.User.Identity.Name);
            await SendChangeOnlineUsers(docId);
        }
        public async Task LeaveDocument(int docId)
        {
            mapper.RemoveUser(docId, Context.User.Identity.Name);
            await SendChangeOnlineUsers(docId);
        }
        private Task SendChangeOnlineUsers(int docId) =>
            Clients.All.SendAsync("ChangeOnlineUsers", mapper.GetUsers(docId));
    }
}
