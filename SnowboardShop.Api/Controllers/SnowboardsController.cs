using Microsoft.AspNetCore.Mvc;
using SnowboardShop.Api.Mapping;
using SnowboardShop.Application.Repositories;
using SnowboardShop.Contracts.Requests;

namespace SnowboardShop.Api.Controllers;

[ApiController]
public class SnowboardsController : ControllerBase
{
    private readonly ISnowboardRepository _snowboardRepository;

    public SnowboardsController(ISnowboardRepository snowboardRepository)
    {
        _snowboardRepository = snowboardRepository;
    }

    [HttpPost(ApiEndpoints.Snowboards.Create)]
    public async Task<IActionResult> Create([FromBody] CreateSnowboardRequest request)
    {
        var snowboard = request.MapToSnowboard();
        
        await _snowboardRepository.CreateAsync(snowboard);
        
        var snowboardResponse = snowboard.MapToResponse();
        
        return CreatedAtAction(nameof(Get), new { id = snowboard.Id }, snowboardResponse);

    }
    
    [HttpGet(ApiEndpoints.Snowboards.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var snowboard = await _snowboardRepository.GetByIdAsync(id);
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
        var snowboards = await _snowboardRepository.GetAllAsync();
        
        var snowboardsResponse = snowboards.MapToResponse();
        
        return Ok(snowboardsResponse);
    }
    
    [HttpPut(ApiEndpoints.Snowboards.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSnowboardRequest request)
    {
        var snowboard = request.MapToSnowboard(id);
        var updatedSnowboard = await _snowboardRepository.UpdateAsync(snowboard);
        if (!updatedSnowboard)
        {
            return NotFound();
        }
        
        var response = snowboard.MapToResponse();
        return Ok(response);
    }
    
    [HttpDelete(ApiEndpoints.Snowboards.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _snowboardRepository.DeleteByIdAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        
        return Ok();
    }
}