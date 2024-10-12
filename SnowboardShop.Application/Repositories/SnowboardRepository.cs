using Dapper;
using SnowboardShop.Application.Database;
using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Repositories;

public class SnowboardRepository : ISnowboardRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SnowboardRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Snowboard snowboard)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into snowboards (id, slug, snowboardbrand, yearofrelease) 
                                                                         values (@Id, @Slug, @SnowboardBrand, @YearOfRelease)
                                                                         """, snowboard));

        if (result > 0)
        {
            foreach (var item in snowboard.SnowboardLineup)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                        insert into snowboardlineup (snowboardid, snowboardmodel) 
                                                                        values (@SnowboardId, @SnowboardModel)
                                                                    """,
                    new { SnowboardId = snowboard.Id, SnowBoardModel = item }));
            }
        }

        transaction.Commit();

        return result > 0;
    }


    public async Task<Snowboard?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var snowboard = await connection.QuerySingleOrDefaultAsync<Snowboard>(
            new CommandDefinition("""
                                  select * from snowboards where id = @id
                                  """, new { id }));

        if (snowboard is null)
        {
            return null;
        }

        var snowboardLineup = await connection.QueryAsync<string>(
            new CommandDefinition("""
                                  select snowboardmodel from snowboardlineup where snowboardid = @id 
                                  """, new { id }));

        foreach (var item in snowboardLineup)
        {
            snowboard.SnowboardLineup.Add(item);
        }

        return snowboard;
    }


    public async Task<Snowboard?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var snowboard = await connection.QuerySingleOrDefaultAsync<Snowboard>(
            new CommandDefinition("""
                                  select * from snowboards where slug = @slug
                                  """, new { slug }));

        if (snowboard is null)
        {
            return null;
        }

        var snowboardLineup = await connection.QueryAsync<string>(new CommandDefinition("""
            select snowboardmodel 
            from snowboardlineup 
            where snowboardid = @id 
            """, new { id = snowboard.Id }));

        foreach (var item in snowboardLineup)
        {
            snowboard.SnowboardLineup.Add(item);
        }

        return snowboard;
    }

    public async Task<IEnumerable<Snowboard>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.QueryAsync<dynamic>(new CommandDefinition("""
                select s.*, string_agg(sl.snowboardmodel, ',') as snowboardlineup
                from snowboards s 
                left join snowboardlineup sl on s.id = sl.snowboardid
                group by s.id
            """));

        return result.Select(x => new Snowboard
        {
            Id = x.id,
            SnowboardBrand = x.snowboardbrand,
            YearOfRelease = x.yearofrelease,
            SnowboardLineup = Enumerable.ToList(x.snowboardlineup.Split(','))
        });
    }


    public async Task<bool> UpdateAsync(Snowboard snowboard)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(
            new CommandDefinition("""
            delete from snowboardlineup where snowboardid = @id
            """, new { id = snowboard.Id }));

        foreach (var item in snowboard.SnowboardLineup)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into snowboardlineup (snowboardid, snowboardmodel) 
                                                                values (@SnowboardId, @SnowboardModel)
                                                                """,
                new { SnowboardId = snowboard.Id, SnowboardModel = item }));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         update snowboards set slug = @Slug, snowboardbrand = @SnowboardBrand, yearofrelease = @YearOfRelease  
                                                                         where id = @Id
                                                                         """, snowboard));

        transaction.Commit();
        return result > 0;
    }


    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from snowboardlineup where snowboardid = @id
                                                            """, new { id }));
        
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from snowboards where id = @id
                                                                         """, new { id }));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               select count(1) from snowboards where id = @id
                                                                               """, new { id }));
    }
}