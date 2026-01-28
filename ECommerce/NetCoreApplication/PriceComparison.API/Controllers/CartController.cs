using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;

namespace PriceComparison.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartSummaryDto>>> GetMyCart()
    {
        var userId = GetUserId();
        var cart = await _cartService.GetUserCartAsync(userId);
        return Ok(ApiResponse<CartSummaryDto>.SuccessResponse(cart));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CartItemDto>>> AddToCart([FromBody] AddToCartDto dto)
    {
        var userId = GetUserId();
        var item = await _cartService.AddToCartAsync(userId, dto);
        return Ok(ApiResponse<CartItemDto>.SuccessResponse(item, "Added to cart"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CartItemDto>>> UpdateCartItem(
        Guid id,
        [FromBody] UpdateCartItemDto dto)
    {
        var item = await _cartService.UpdateCartItemAsync(id, dto);
        if (item == null)
        {
            if (dto.Quantity > 0)
            {
                return NotFound(ApiResponse<CartItemDto>.FailResponse("Cart item not found"));
            }
            return Ok(ApiResponse.SuccessResponse("Cart item removed"));
        }

        return Ok(ApiResponse<CartItemDto>.SuccessResponse(item, "Cart item updated"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> RemoveFromCart(Guid id)
    {
        var result = await _cartService.RemoveFromCartAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse.FailResponse("Cart item not found"));
        }

        return Ok(ApiResponse.SuccessResponse("Removed from cart"));
    }

    [HttpDelete]
    public async Task<ActionResult<ApiResponse>> ClearCart()
    {
        var userId = GetUserId();
        await _cartService.ClearCartAsync(userId);
        return Ok(ApiResponse.SuccessResponse("Cart cleared"));
    }
}
