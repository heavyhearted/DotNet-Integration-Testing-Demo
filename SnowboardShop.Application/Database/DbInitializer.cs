using Dapper;

namespace SnowboardShop.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync("""
                                          create table if not exists snowboards (
                                          id UUID primary key,
                                          slug TEXT not null, 
                                          snowboardbrand TEXT not null,
                                          yearofrelease integer not null);
                                      """);
        
        await connection.ExecuteAsync("""
                                          create unique index concurrently if not exists snowboards_slug_idx
                                          on snowboards
                                          using btree(slug);
                                      """);
        
        await connection.ExecuteAsync("""
                                          create table if not exists snowboardlineup (
                                          snowboardId UUID references snowboards (Id),
                                          snowboardmodel TEXT not null);
                                      """);
    }
}
