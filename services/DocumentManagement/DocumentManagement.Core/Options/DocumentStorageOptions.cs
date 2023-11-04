using Storage.Net.Blobs;
using Storage.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Core.Options
{
    public class DocumentStorageOptions
    {
        public Func<IBlobStorage> BlobStorageFactory { get; set; } = StorageFactory.Blobs.InMemory;
    }
}
