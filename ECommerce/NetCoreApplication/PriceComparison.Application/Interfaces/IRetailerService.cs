using PriceComparison.Application.DTOs;

namespace PriceComparison.Application.Interfaces;

public interface IRetailerService
{
    Task<List<RetailerDto>> GetAllAsync();
    Task<List<RetailerDto>> GetActiveAsync();
    Task<RetailerDto?> GetByIdAsync(Guid id);
    Task<RetailerDto> CreateAsync(CreateRetailerDto dto);
    Task<RetailerDto?> UpdateAsync(Guid id, UpdateRetailerDto dto);
    Task<bool> DeleteAsync(Guid id);
}
