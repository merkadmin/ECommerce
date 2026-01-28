using AutoMapper;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;

namespace PriceComparison.Application.Services;

public class RetailerService : IRetailerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RetailerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<RetailerDto>> GetAllAsync()
    {
        var retailers = await _unitOfWork.Retailers.GetAllAsync();
        return _mapper.Map<List<RetailerDto>>(retailers);
    }

    public async Task<List<RetailerDto>> GetActiveAsync()
    {
        var retailers = await _unitOfWork.Retailers.GetActiveRetailersAsync();
        return _mapper.Map<List<RetailerDto>>(retailers);
    }

    public async Task<RetailerDto?> GetByIdAsync(Guid id)
    {
        var retailer = await _unitOfWork.Retailers.GetByIdAsync(id);
        return retailer == null ? null : _mapper.Map<RetailerDto>(retailer);
    }

    public async Task<RetailerDto> CreateAsync(CreateRetailerDto dto)
    {
        var retailer = _mapper.Map<Retailer>(dto);
        retailer.Id = Guid.NewGuid();
        retailer.IsActive = true;
        retailer.CreatedAt = DateTime.UtcNow;
        retailer.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Retailers.AddAsync(retailer);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RetailerDto>(retailer);
    }

    public async Task<RetailerDto?> UpdateAsync(Guid id, UpdateRetailerDto dto)
    {
        var retailer = await _unitOfWork.Retailers.GetByIdAsync(id);
        if (retailer == null) return null;

        if (dto.Name != null) retailer.Name = dto.Name;
        if (dto.LogoUrl != null) retailer.LogoUrl = dto.LogoUrl;
        if (dto.BaseUrl != null) retailer.BaseUrl = dto.BaseUrl;
        if (dto.AverageRating.HasValue) retailer.AverageRating = dto.AverageRating;
        if (dto.IsActive.HasValue) retailer.IsActive = dto.IsActive.Value;

        retailer.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Retailers.Update(retailer);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RetailerDto>(retailer);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var retailer = await _unitOfWork.Retailers.GetByIdAsync(id);
        if (retailer == null) return false;

        _unitOfWork.Retailers.Remove(retailer);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
