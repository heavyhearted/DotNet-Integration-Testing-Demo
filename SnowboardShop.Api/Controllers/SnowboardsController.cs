using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnowboardShop.Api.Auth;
using SnowboardShop.Api.Mapping;
using SnowboardShop.Application.Services;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Controllers;

[ApiController]
public class SnowboardsController : ControllerBase
{
    private readonly ISnowboardService _snowboardService;

    public SnowboardsController(ISnowboardService snowboardService)
    {
        _snowboardService = snowboardService;
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Snowboards.Create)]
    public async Task<IActionResult> Create([FromBody] CreateSnowboardRequest request, CancellationToken token)
    {
        var snowboard = request.MapToSnowboard();

        await _snowboardService.CreateAsync(snowboard, token);

        var snowboardResponse = snowboard.MapToResponse();

        return CreatedAtAction(nameof(Get), new { idOrSlug = snowboard.Id }, snowboardResponse);
    }

    [HttpGet(ApiEndpoints.Snowboards.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var snowboard = Guid.TryParse(idOrSlug, out var id)
            ? await _snowboardService.GetByIdAsync(id, token)
            : await _snowboardService.GetBySlugAsync(idOrSlug, token);
        if (snowboard is null)
        {
            return NotFound();
        }

        var response = snowboard.MapToResponse();
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Snowboards.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var userId = HttpContext.User;
        var snowboards = await _snowboardService.GetAllAsync(token);

        var snowboardsResponse = snowboards.MapToResponse();

        return Ok(snowboardsResponse);
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Snowboards.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSnowboardRequest request,
        CancellationToken token)
    {
        var snowboard = request.MapToSnowboard(id);
        var updatedSnowboard = await _snowboardService.UpdateAsync(snowboard, token);
        if (updatedSnowboard is null)
        {
            return NotFound();
        }

        var response = updatedSnowboard.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Snowboards.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _snowboardService.DeleteByIdAsync(id, token);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}