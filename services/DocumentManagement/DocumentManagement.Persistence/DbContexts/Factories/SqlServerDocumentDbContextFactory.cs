using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DocumentManagement.Persistence.DbContexts.Factories;

public class SqlServerDocumentDbContextFactory : IDesignTimeDbContextFactory<DocumentDbContext>
{
    public DocumentDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<DocumentDbContext>();
        var connectionString = "Data Source=elsa.sqlite.db;";

        builder.UseSqlite(connectionString);

        return new DocumentDbContext(builder.Options);
    }
}