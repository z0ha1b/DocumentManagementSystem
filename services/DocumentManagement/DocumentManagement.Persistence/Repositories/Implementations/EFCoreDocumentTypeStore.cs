using DocumentManagement.Core.Models;
using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Persistence.Repositories.Implementations;

public class EFCoreDocumentTypeStore : IDocumentTypeStore
{
    private readonly IDbContextFactory<DocumentDbContext> _dbContextFactory;

    public EFCoreDocumentTypeStore(IDbContextFactory<DocumentDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<DocumentType>> ListAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.DocumentTypes.ToListAsync(cancellationToken);
    }

    public async Task<DocumentType?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.DocumentTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
