using BlogCore.DAL.Data;
using BlogCore.DAL.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;

namespace Tests;

[TestClass]
public abstract class IntegrationTestBase
{
    protected static readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("StrongPassword123!")
        .Build();

    private static Respawner _respawner = null!;
    private static string _connectionString = null!;

    protected BlogContext _context = null!;
    protected BlogRepository _repository = null!;

    [AssemblyInitialize]
    public static async Task AssemblyInit(TestContext context)
    {
        await _dbContainer.StartAsync();

        _connectionString = _dbContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<BlogContext>()
            .UseSqlServer(_connectionString)
            .Options;

        using (var dbContext = new BlogContext(options))
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                TablesToIgnore = new Table[]
                {
                    new Table("__EFMigrationsHistory")
                }
            });
        }
    }

    [TestInitialize]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<BlogContext>()
            .UseSqlServer(_connectionString)
            .Options;

        _context = new BlogContext(options);
        _repository = new BlogRepository(_context);

        await ResetDatabaseAsync();
    }

    protected async Task ResetDatabaseAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            await _respawner.ResetAsync(connection);
        }
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [AssemblyCleanup]
    public static async Task AssemblyCleanup()
    {
        await _dbContainer.StopAsync();
    }
}