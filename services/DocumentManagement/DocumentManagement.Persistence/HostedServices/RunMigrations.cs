using DocumentManagement.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DocumentManagement.Persistence.HostedServices;

public class RunMigrations : IHostedService
{
    private readonly IDbContextFactory<DocumentDbContext> _dbContextFactory;

    public RunMigrations(IDbContextFactory<DocumentDbContext> dbContextFactoryFactory)
    {
        _dbContextFactory = dbContextFactoryFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.Database.MigrateAsync(cancellationToken);
        await dbContext.DisposeAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}