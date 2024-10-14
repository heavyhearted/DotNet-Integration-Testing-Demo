using Dapper;
using SnowboardShop.Application.Database;
using SnowboardShop.Application.Models;

namespace SnowboardShop.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    public async Task<bool> RateSnowboardAsync(Guid snowboardId, int rating, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into ratings(userid, snowboardid, rating) 
                                                                         values (@userId, @snowboardId, @rating)
                                                                         on conflict (userid, snowboardid) do update 
                                                                             set rating = @rating
                                                                         """, new { userId, snowboardId, rating }, cancellationToken: token));

        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid snowboardId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            select round(avg(r.rating), 1) from ratings r
            where snowboardid = @snowboardId
            """, new { snowboardId }, cancellationToken: token));
    }


    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid snowboardId, Guid userId,
        CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select round(avg(rating), 1), 
                   (select rating 
                    from ratings 
                    where snowboardid = @snowboardId 
                      and userid = @userId
                    limit 1) 
            from ratings
            where snowboardid = @snowboardId
            """, new { snowboardId, userId }, cancellationToken: token));
    }

    public async Task<bool> DeleteRatingAsync(Guid snowboardId, Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from ratings
                                                                         where snowboardid = @snowboardId
                                                                         and userid = @userId
                                                                         """, new { userId, snowboardId }, cancellationToken: token));

        return result > 0;
    }

    public async Task<IEnumerable<SnowboardRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QueryAsync<SnowboardRating>(new CommandDefinition("""
            select r.rating, r.snowboardid, s.slug
            from ratings r
            inner join snowboards s on r.snowboardid = s.id
            where userid = @userId
            """, new { userId }, cancellationToken: token));
    }
}