using Microsoft.AspNetCore.Mvc;
using SnowboardShop.Api.Mapping;
using SnowboardShop.Application.Repositories;
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


    [HttpPost(ApiEndpoints.Snowboards.Create)]
    public async Task<IActionResult> Create([FromBody] CreateSnowboardRequest request)
    {
        var snowboard = request.MapToSnowboard();
        
        await _snowboardService.CreateAsync(snowboard);
        
        var snowboardResponse = snowboard.MapToResponse();
        
        return CreatedAtAction(nameof(Get), new { idOrSlug = snowboard.Id }, snowboardResponse);

    }
    
    [HttpGet(ApiEndpoints.Snowboards.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    { 
       var snowboard = Guid.TryParse(idOrSlug, out var id)
            ? await _snowboardService.GetByIdAsync(id)
            : await _snowboardService.GetBySlugAsync(idOrSlug);
        if (snowboard is null)
        {
            return NotFound();
        }

        var response = snowboard.MapToResponse();
        return Ok(response);
    }
    
    [HttpGet(ApiEndpoints.Snowboards.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var snowboards = await _snowboardService.GetAllAsync();
        
        var snowboardsResponse = snowboards.MapToResponse();
        
        return Ok(snowboardsResponse);
    }
    
    [HttpPut(ApiEndpoints.Snowboards.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSnowboardRequest request)
    {
        var snowboard = request.MapToSnowboard(id);
        var updatedSnowboard = await _snowboardService.UpdateAsync(snowboard);
        if (updatedSnowboard is null)
        {
            return NotFound();
        }
        
        var response = updatedSnowboard.MapToResponse();
        return Ok(response);
    }
    
    [HttpDelete(ApiEndpoints.Snowboards.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _snowboardService.DeleteByIdAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        
        return Ok();
    }
}