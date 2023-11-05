using DocumentManagement.Core.Models;
using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Persistence.Repositories.Implementations;

public class EFCoreDocumentStore : IDocumentStore
{
    private readonly IDbContextFactory<DocumentDbContext> _dbContextFactory;

    public EFCoreDocumentStore(IDbContextFactory<DocumentDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task SaveAsync(Document entity, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existingDocument = await dbContext.Documents.FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

        if (existingDocument == null)
            await dbContext.Documents.AddAsync(entity, cancellationToken);
        else
            dbContext.Entry(existingDocument).CurrentValues.SetValues(entity);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Document?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Documents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Document?>> GetDocsAsync(string batchId, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var docs = dbContext.Documents.Where(x => x.BatchId == batchId).ToList();
        return docs;
    }
}