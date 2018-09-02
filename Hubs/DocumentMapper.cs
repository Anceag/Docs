using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Docs.Hubs
{
    public class DocumentMapper
    {
        private readonly Dictionary<int, HashSet<string>> documentsUsers =
            new Dictionary<int, HashSet<string>>();

        public void AddUser(int documentId, string userName)
        {
            lock (documentsUsers)
            {
                if (!documentsUsers.TryGetValue(documentId, out HashSet<string> users))
                {
                    users = new HashSet<string>();
                    documentsUsers.Add(documentId, users);
                }

                lock (users)
                {
                    users.Add(userName);
                }
            }
        }

        public IEnumerable<string> GetUsers(int documentId)
        {
            if (documentsUsers.TryGetValue(documentId, out HashSet<string> users))
            {
                return users;
            }

            return Enumerable.Empty<string>();
        }

        public void RemoveUser(int documentId, string userName)
        {
            lock (documentsUsers)
            {
                if (!documentsUsers.TryGetValue(documentId, out HashSet<string> users))
                {
                    return;
                }

                lock (users)
                {
                    users.Remove(userName);

                    if (users.Count == 0)
                    {
                        documentsUsers.Remove(documentId);
                    }
                }
            }
        }
    }
}
