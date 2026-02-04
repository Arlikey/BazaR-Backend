using Microsoft.EntityFrameworkCore;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;

namespace BazaR.Backend.UnitTests.Helpers;

public class TestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}