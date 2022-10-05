using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infrastructure.Repositories;

public class DatabaseContextFactory
{
    private readonly Action<DbContextOptionsBuilder> _configurationDbContext;
    public DatabaseContextFactory(Action<DbContextOptionsBuilder> configurationDbContext)
    {
        _configurationDbContext = configurationDbContext;
    }

    public DatabaseContext CreateDbContext()
    {
        DbContextOptionsBuilder<DatabaseContext> optionsBuilder = new();

        _configurationDbContext(optionsBuilder);

        return new DatabaseContext(optionsBuilder.Options);
    }
}
