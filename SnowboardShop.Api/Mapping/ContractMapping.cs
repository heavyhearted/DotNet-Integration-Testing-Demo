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
            SnowboardProfile = request.SnowboardProfile.ToList()
        };
    }
    
    public static Snowboard MapToSnowboard(this UpdateSnowboardRequest request, Guid id)
    {
        return new Snowboard
        {
            Id = id,
            SnowboardBrand = request.SnowboardBrand,
            YearOfRelease = request.YearOfRelease,
            SnowboardProfile = request.SnowboardProfile.ToList()
        };
    }
    
    public static SnowboardResponse MapToResponse(this Snowboard snowboard)
    {
        return new SnowboardResponse
        {
            Id = snowboard.Id,
            SnowboardBrand = snowboard.SnowboardBrand,
            YearOfRelease = snowboard.YearOfRelease,
            SnowboardProfile = snowboard.SnowboardProfile
        };
    }
    
    public static SnowboardsResponse MapToResponse(this IEnumerable<Snowboard> snowboards)
    {
        return new SnowboardsResponse
        {
            Items = snowboards.Select(MapToResponse)
        };
    }
}


