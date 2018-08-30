using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Docs.Hubs
{
    public class DocsHub : Hub
    {
        public async Task TextChange(string message)
        {
            await Clients.Others.SendAsync("TextChange", message);
        }
    }
}
