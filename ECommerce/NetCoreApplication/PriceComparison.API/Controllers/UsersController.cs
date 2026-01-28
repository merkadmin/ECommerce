using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;

namespace PriceComparison.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
    {
        var userId = GetUserId();
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound(ApiResponse<UserDto>.FailResponse("User not found"));
        }

        return Ok(ApiResponse<UserDto>.SuccessResponse(user));
    }

    [HttpPut("me")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateCurrentUser([FromBody] UpdateUserDto dto)
    {
        var userId = GetUserId();
        var user = await _userService.UpdateAsync(userId, dto);
        if (user == null)
        {
            return NotFound(ApiResponse<UserDto>.FailResponse("User not found"));
        }

        return Ok(ApiResponse<UserDto>.SuccessResponse(user, "Profile updated successfully"));
    }

    [HttpDelete("me")]
    public async Task<ActionResult<ApiResponse>> DeleteCurrentUser()
    {
        var userId = GetUserId();
        var result = await _userService.DeleteAsync(userId);
        if (!result)
        {
            return NotFound(ApiResponse.FailResponse("User not found"));
        }

        return Ok(ApiResponse.SuccessResponse("Account deactivated successfully"));
    }
}
