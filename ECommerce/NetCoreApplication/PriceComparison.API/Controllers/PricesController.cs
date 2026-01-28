using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;

namespace PriceComparison.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricesController : ControllerBase
{
    private readonly IPriceService _priceService;

    public PricesController(IPriceService priceService)
    {
        _priceService = priceService;
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<ApiResponse<List<PriceDto>>>> GetPricesByProduct(Guid productId)
    {
        var prices = await _priceService.GetByProductIdAsync(productId);
        return Ok(ApiResponse<List<PriceDto>>.SuccessResponse(prices));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PriceDto>>> GetPrice(Guid id)
    {
        var price = await _priceService.GetByIdAsync(id);
        if (price == null)
        {
            return NotFound(ApiResponse<PriceDto>.FailResponse("Price not found"));
        }

        return Ok(ApiResponse<PriceDto>.SuccessResponse(price));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PriceDto>>> CreatePrice([FromBody] CreatePriceDto dto)
    {
        var price = await _priceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetPrice), new { id = price.Id },
            ApiResponse<PriceDto>.SuccessResponse(price, "Price created successfully"));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PriceDto>>> UpdatePrice(Guid id, [FromBody] UpdatePriceDto dto)
    {
        var price = await _priceService.UpdateAsync(id, dto);
        if (price == null)
        {
            return NotFound(ApiResponse<PriceDto>.FailResponse("Price not found"));
        }

        return Ok(ApiResponse<PriceDto>.SuccessResponse(price, "Price updated successfully"));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeletePrice(Guid id)
    {
        var result = await _priceService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse.FailResponse("Price not found"));
        }

        return Ok(ApiResponse.SuccessResponse("Price deleted successfully"));
    }
}
