using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    Task<bool> LogoutAsync(Guid userId);
}
