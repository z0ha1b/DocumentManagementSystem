using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DocumentManagement.Persistence.DbContexts.Factories;

public class SqlServerDocumentDbContextFactory : IDesignTimeDbContextFactory<DocumentDbContext>
{
    public DocumentDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<DocumentDbContext>();
        var connectionString = "Server=.;Database=Elsa;TrustServerCertificate=true;Trusted_Connection=True;MultipleActiveResultSets=true";

        builder.UseSqlServer(connectionString);

        return new DocumentDbContext(builder.Options);
    }
}