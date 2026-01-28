using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;

namespace PriceComparison.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<WishlistItemDto>>>> GetMyWishlist()
    {
        var userId = GetUserId();
        var items = await _wishlistService.GetUserWishlistAsync(userId);
        return Ok(ApiResponse<List<WishlistItemDto>>.SuccessResponse(items));
    }

    [HttpGet("check/{productId:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> IsInWishlist(Guid productId)
    {
        var userId = GetUserId();
        var result = await _wishlistService.IsInWishlistAsync(userId, productId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<WishlistItemDto>>> AddToWishlist([FromBody] AddToWishlistDto dto)
    {
        var userId = GetUserId();
        var item = await _wishlistService.AddToWishlistAsync(userId, dto);
        return Ok(ApiResponse<WishlistItemDto>.SuccessResponse(item, "Added to wishlist"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<WishlistItemDto>>> UpdateWishlistItem(
        Guid id,
        [FromBody] UpdateWishlistItemDto dto)
    {
        var item = await _wishlistService.UpdateAsync(id, dto);
        if (item == null)
        {
            return NotFound(ApiResponse<WishlistItemDto>.FailResponse("Wishlist item not found"));
        }

        return Ok(ApiResponse<WishlistItemDto>.SuccessResponse(item, "Wishlist item updated"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> RemoveFromWishlist(Guid id)
    {
        var result = await _wishlistService.RemoveFromWishlistAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse.FailResponse("Wishlist item not found"));
        }

        return Ok(ApiResponse.SuccessResponse("Removed from wishlist"));
    }
}
