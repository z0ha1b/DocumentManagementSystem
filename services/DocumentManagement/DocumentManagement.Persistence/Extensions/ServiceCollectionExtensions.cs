using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Persistence.DbContexts;
using DocumentManagement.Persistence.DbContexts.Factories;
using DocumentManagement.Persistence.HostedServices;
using DocumentManagement.Persistence.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentManagement.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainPersistence(this IServiceCollection services, string connectionString)
    {
        var migrationsAssemblyName = typeof(SqlServerDocumentDbContextFactory).Assembly.GetName().Name;

        return services
            .AddPooledDbContextFactory<DocumentDbContext>(x => x.UseSqlite(connectionString, db => db.MigrationsAssembly(migrationsAssemblyName)))
            .AddSingleton<IDocumentStore, EFCoreDocumentStore>()
            .AddSingleton<IDocumentTypeStore, EFCoreDocumentTypeStore>()
            .AddHostedService<RunMigrations>();
    }
}