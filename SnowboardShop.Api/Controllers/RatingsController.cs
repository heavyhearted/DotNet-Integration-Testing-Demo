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
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.RateSnowboardAsync(id, request.Rating, userId!.Value, token);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Snowboards.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.DeleteRatingAsync(id, userId!.Value, token);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> GetUserRatings(CancellationToken token = default)
    {
        var userId = HttpContext.GetUserId();
        var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, token);
        var ratingsResponse = ratings.MapToResponse();
        return Ok(ratingsResponse);
    }
}