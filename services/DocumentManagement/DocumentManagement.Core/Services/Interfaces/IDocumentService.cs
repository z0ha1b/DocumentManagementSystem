using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManagement.Core.Models;

namespace DocumentManagement.Core.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<Document> SaveDocumentAsync(string fileName, Stream data, string documentTypeId, CancellationToken cancellationToken = default);
    }
}