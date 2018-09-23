using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Docs.Hubs.DocumentHubMapper
{
    public class DocumentMapper
    {
        private readonly Dictionary<int, List<DocumentMapperUser>> documentsUsers =
            new Dictionary<int, List<DocumentMapperUser>>();

        public void AddUser(int documentId, DocumentMapperUser user)
        {
            lock (documentsUsers)
            {
                if (!documentsUsers.TryGetValue(documentId, out List<DocumentMapperUser> users))
                {
                    users = new List<DocumentMapperUser>();
                    documentsUsers.Add(documentId, users);
                }

                lock (users)
                {
                    users.Add(user);
                }
            }
        }

        public IEnumerable<DocumentMapperUser> GetUsers(int documentId)
        {
            if (documentsUsers.TryGetValue(documentId, out List<DocumentMapperUser> users))
            {
                return users;
            }

            return Enumerable.Empty<DocumentMapperUser>();
        }

        public void RemoveUser(int documentId, string userName)
        {
            lock (documentsUsers)
            {
                if (!documentsUsers.TryGetValue(documentId, out List<DocumentMapperUser> users))
                {
                    return;
                }

                lock (users)
                {
                    users.Remove(users.First(u => u.Name == userName));

                    if (users.Count == 0)
                    {
                        documentsUsers.Remove(documentId);
                    }
                }
            }
        }

        public int FindDocumentIdByUserName(string userName)
        {
            return documentsUsers.FirstOrDefault(du => du.Value.FirstOrDefault(u => u.Name == userName) != null).Key;
        }
    }
}
