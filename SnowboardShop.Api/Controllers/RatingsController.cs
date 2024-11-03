using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnowboardShop.Api.Auth;
using SnowboardShop.Api.Mapping;
using SnowboardShop.Application.Services;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Controllers;

[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [Authorize]
    [HttpPut(ApiEndpoints.Snowboards.Rate)]
    public async Task<IActionResult> RateSnowboard([FromRoute] Guid id,
        [FromBody] RateSnowboardRequest request, CancellationToken token)
    {
        return await _ratingService.RateSnowboardAsync(id, request.Rating, token)
            ? Ok(new { Message = "Rating submitted successfully", SnowboardId = id, Rating = request.Rating })
            : NotFound(new { Message = "The requested snowboard was not found" });
    }


    [Authorize]
    [HttpDelete(ApiEndpoints.Snowboards.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id,
        CancellationToken token)
    {
        return await _ratingService.DeleteRatingAsync(id, token)
            ? Ok(new { Message = "Rating deleted successfully"})
            : NotFound(new { Message = "The requested snowboard was not found" });
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> GetUserRatings(CancellationToken token = default)
    {
        var ratings = await _ratingService.GetRatingsForUserAsync(token);
        var ratingsResponse = ratings.MapToResponse();
        return Ok(ratingsResponse);
    }
}