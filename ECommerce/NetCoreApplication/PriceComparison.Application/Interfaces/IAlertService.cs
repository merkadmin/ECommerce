using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Interfaces;

public interface IAlertService
{
    Task<List<PriceAlertDto>> GetUserAlertsAsync(Guid userId);
    Task<PriceAlertDto?> GetByIdAsync(Guid id);
    Task<PriceAlertDto> CreateAsync(Guid userId, CreateAlertDto dto);
    Task<PriceAlertDto?> UpdateAsync(Guid id, UpdateAlertDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task CheckAndTriggerAlertsAsync();
}
