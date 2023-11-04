using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Repositories.Interfaces
{
    public interface IDocumentStore
    {
        Task SaveAsync(Document entity, CancellationToken cancellationToken = default);
        Task<Document?> GetAsync(string id, CancellationToken cancellationToken = default);
    }
}