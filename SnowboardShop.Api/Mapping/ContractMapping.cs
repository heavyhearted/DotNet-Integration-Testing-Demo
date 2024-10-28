using SnowboardShop.Application.Models;
using SnowboardShop.Contracts.Requests;
using SnowboardShop.Contracts.Responses;

namespace SnowboardShop.Api.Mapping;

public static class ContractMapping
{
    public static Snowboard MapToSnowboard(this CreateSnowboardRequest request)
    {
        return new Snowboard
        {
            Id = Guid.NewGuid(),
            SnowboardBrand = request.SnowboardBrand,
            YearOfRelease = request.YearOfRelease,
            SnowboardLineup = request.SnowboardLineup.ToList()
        };
    }
    
    public static Snowboard MapToSnowboard(this UpdateSnowboardRequest request, Guid id)
    {
        return new Snowboard
        {
            Id = id,
            SnowboardBrand = request.SnowboardBrand,
            YearOfRelease = request.YearOfRelease,
            SnowboardLineup = request.SnowboardLineup.ToList()
        };
    }
    
    public static SnowboardResponse MapToResponse(this Snowboard snowboard)
    {
        return new SnowboardResponse
        {
            Id = snowboard.Id,
            SnowboardBrand = snowboard.SnowboardBrand,
            Slug = snowboard.Slug,
            YearOfRelease = snowboard.YearOfRelease,
            SnowboardLineup = snowboard.SnowboardLineup
        };
    }
    
    public static SnowboardsResponse MapToResponse(this IEnumerable<Snowboard> snowboards)
    {
        return new SnowboardsResponse
        {
            Items = snowboards.Select(MapToResponse)
        };
    }
    
    public static IEnumerable<SnowboardRatingsResponse> MapToResponse(this IEnumerable<SnowboardRating> ratings)
    {
        return ratings.Select(x => new SnowboardRatingsResponse
        {
            Rating = x.Rating,
            Slug = x.Slug,
            SnowboardId = x.SnowboardId
        });
    }
}


