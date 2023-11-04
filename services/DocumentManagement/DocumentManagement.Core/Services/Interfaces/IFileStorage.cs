using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Core.Services.Interfaces
{
    public interface IFileStorage
    {
        Task WriteAsync(Stream data, string fileName, CancellationToken cancellationToken = default);
        Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
