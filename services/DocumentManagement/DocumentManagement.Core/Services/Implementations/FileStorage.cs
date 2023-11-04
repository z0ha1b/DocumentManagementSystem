using DocumentManagement.Core.Services.Interfaces;
using Microsoft.Extensions.Options;
using Storage.Net.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManagement.Core.Options;

namespace DocumentManagement.Core.Services.Implementations
{
    public class FileStorage : IFileStorage
    {
        private readonly IBlobStorage _blobStorage;

        public FileStorage(IOptions<DocumentStorageOptions> options) => _blobStorage = options.Value.BlobStorageFactory();

        public Task WriteAsync(Stream data, string fileName, CancellationToken cancellationToken = default) =>
            _blobStorage.WriteAsync(fileName, data, false, cancellationToken);

        public Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default) =>
            _blobStorage.OpenReadAsync(fileName, cancellationToken);
    }
}