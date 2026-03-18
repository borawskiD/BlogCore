using BlogCore.DAL.Data;
using BlogCore.DAL.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Tests;

[TestClass]
public abstract class IntegrationTestBase
{
    protected static readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("StrongPassword123!")
        .Build();

    private string _connectionString = null!;

    protected BlogContext _context = null!;
    protected BlogRepository _repository = null!;

    [AssemblyInitialize]
    public static async Task AssemblyInit(TestContext context)
    {
        await _dbContainer.StartAsync();
    }

    [TestInitialize]
    public async Task Setup()
    {
        var dbName = $"BlogCoreTests_{Guid.NewGuid():N}";

        var builder = new SqlConnectionStringBuilder(_dbContainer.GetConnectionString())
        {
            InitialCatalog = dbName
        };

        _connectionString = builder.ConnectionString;

        var options = new DbContextOptionsBuilder<BlogContext>()
            .UseSqlServer(_connectionString)
            .Options;

        _context = new BlogContext(options);
        await _context.Database.EnsureCreatedAsync();

        _repository = new BlogRepository(_context);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        if (_context != null)
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
    }

    [AssemblyCleanup]
    public static async Task AssemblyCleanup()
    {
        await _dbContainer.StopAsync();
    }
}