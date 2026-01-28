using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;

namespace PriceComparison.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RetailersController : ControllerBase
{
    private readonly IRetailerService _retailerService;

    public RetailersController(IRetailerService retailerService)
    {
        _retailerService = retailerService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RetailerDto>>>> GetRetailers()
    {
        var retailers = await _retailerService.GetActiveAsync();
        return Ok(ApiResponse<List<RetailerDto>>.SuccessResponse(retailers));
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<List<RetailerDto>>>> GetAllRetailers()
    {
        var retailers = await _retailerService.GetAllAsync();
        return Ok(ApiResponse<List<RetailerDto>>.SuccessResponse(retailers));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RetailerDto>>> GetRetailer(Guid id)
    {
        var retailer = await _retailerService.GetByIdAsync(id);
        if (retailer == null)
        {
            return NotFound(ApiResponse<RetailerDto>.FailResponse("Retailer not found"));
        }

        return Ok(ApiResponse<RetailerDto>.SuccessResponse(retailer));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RetailerDto>>> CreateRetailer([FromBody] CreateRetailerDto dto)
    {
        var retailer = await _retailerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetRetailer), new { id = retailer.Id },
            ApiResponse<RetailerDto>.SuccessResponse(retailer, "Retailer created successfully"));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RetailerDto>>> UpdateRetailer(Guid id, [FromBody] UpdateRetailerDto dto)
    {
        var retailer = await _retailerService.UpdateAsync(id, dto);
        if (retailer == null)
        {
            return NotFound(ApiResponse<RetailerDto>.FailResponse("Retailer not found"));
        }

        return Ok(ApiResponse<RetailerDto>.SuccessResponse(retailer, "Retailer updated successfully"));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteRetailer(Guid id)
    {
        var result = await _retailerService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse.FailResponse("Retailer not found"));
        }

        return Ok(ApiResponse.SuccessResponse("Retailer deleted successfully"));
    }
}
