using Dapper;
using SnowboardShop.Application.Database;

namespace SnowboardShop.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
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
}