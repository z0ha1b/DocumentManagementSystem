using System.Reflection;
using DocumentManagement.Core.Options;
using DocumentManagement.Core.Services.Implementations;
using DocumentManagement.Core.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Storage.Net;

namespace DocumentManagement.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = StorageFactory.Blobs.InMemory);
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


        return services
            .AddSingleton<ISystemClock, SystemClock>()
            .AddSingleton<IFileStorage, FileStorage>()
            .AddScoped<IDocumentService, DocumentService>();
    }
}