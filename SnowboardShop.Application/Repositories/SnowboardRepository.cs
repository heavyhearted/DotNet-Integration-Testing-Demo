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

    public async Task<bool> CreateAsync(Snowboard snowboard, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into snowboards (id, slug, snowboardbrand, yearofrelease) 
                                                                         values (@Id, @Slug, @SnowboardBrand, @YearOfRelease)
                                                                         """, snowboard, cancellationToken: token));

        if (result > 0)
        {
            foreach (var item in snowboard.SnowboardLineup)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                        insert into snowboardlineup (snowboardid, snowboardmodel) 
                                                                        values (@SnowboardId, @SnowboardModel)
                                                                    """,
                    new { SnowboardId = snowboard.Id, SnowBoardModel = item }, cancellationToken: token));
            }
        }

        transaction.Commit();

        return result > 0;
    }


    public async Task<Snowboard?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var snowboard = await connection.QuerySingleOrDefaultAsync<Snowboard>(
            new CommandDefinition("""
                                  select * from snowboards where id = @id
                                  """, new { id }, cancellationToken: token));

        if (snowboard is null)
        {
            return null;
        }

        var snowboardLineup = await connection.QueryAsync<string>(
            new CommandDefinition("""
                                  select snowboardmodel from snowboardlineup where snowboardid = @id 
                                  """, new { id }, cancellationToken: token));

        foreach (var item in snowboardLineup)
        {
            snowboard.SnowboardLineup.Add(item);
        }

        return snowboard;
    }


    public async Task<Snowboard?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var snowboard = await connection.QuerySingleOrDefaultAsync<Snowboard>(
            new CommandDefinition("""
                                  select * from snowboards where slug = @slug
                                  """, new { slug }, cancellationToken: token));

        if (snowboard is null)
        {
            return null;
        }

        var snowboardLineup = await connection.QueryAsync<string>(new CommandDefinition("""
            select snowboardmodel 
            from snowboardlineup 
            where snowboardid = @id 
            """, new { id = snowboard.Id }, cancellationToken: token));

        foreach (var item in snowboardLineup)
        {
            snowboard.SnowboardLineup.Add(item);
        }

        return snowboard;
    }

    public async Task<IEnumerable<Snowboard>> GetAllAsync(CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.QueryAsync<dynamic>(new CommandDefinition("""
                select s.*, string_agg(sl.snowboardmodel, ',') as snowboardlineup
                from snowboards s 
                left join snowboardlineup sl on s.id = sl.snowboardid
                group by s.id
            """, cancellationToken: token));

        return result.Select(x => new Snowboard
        {
            Id = x.id,
            SnowboardBrand = x.snowboardbrand,
            YearOfRelease = x.yearofrelease,
            SnowboardLineup = Enumerable.ToList(x.snowboardlineup.Split(','))
        });
    }


    public async Task<bool> UpdateAsync(Snowboard snowboard, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(
            new CommandDefinition("""
            delete from snowboardlineup where snowboardid = @id
            """, new { id = snowboard.Id }, cancellationToken: token));

        foreach (var item in snowboard.SnowboardLineup)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                insert into snowboardlineup (snowboardid, snowboardmodel) 
                                                                values (@SnowboardId, @SnowboardModel)
                                                                """,
                new { SnowboardId = snowboard.Id, SnowboardModel = item }, cancellationToken: token));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         update snowboards set slug = @Slug, snowboardbrand = @SnowboardBrand, yearofrelease = @YearOfRelease  
                                                                         where id = @Id
                                                                         """, snowboard, cancellationToken: token));

        transaction.Commit();
        return result > 0;
    }


    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from snowboardlineup where snowboardid = @id
                                                            """, new { id }, cancellationToken: token));
        
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from snowboards where id = @id
                                                                         """, new { id }, cancellationToken: token));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               select count(1) from snowboards where id = @id
                                                                               """, new { id }, cancellationToken: token));
    }
}