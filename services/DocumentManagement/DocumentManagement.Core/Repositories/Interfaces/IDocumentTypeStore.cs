using DocumentManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Core.Repositories.Interfaces
{
    public interface IDocumentTypeStore
    {
        Task<IEnumerable<DocumentType>> ListAsync(CancellationToken cancellationToken = default);
        Task<DocumentType?> GetAsync(string id, CancellationToken cancellationToken = default);
    }
}
