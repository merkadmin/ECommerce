using AutoMapper;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;
using PriceComparison.Core.Entities;
using PriceComparison.Core.Interfaces;

namespace PriceComparison.Application.Services;

public class AlertService : IAlertService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPriceComparisonService _priceComparisonService;
    private readonly INotificationService _notificationService;

    public AlertService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPriceComparisonService priceComparisonService,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _priceComparisonService = priceComparisonService;
        _notificationService = notificationService;
    }

    public async Task<List<PriceAlertDto>> GetUserAlertsAsync(Guid userId)
    {
        var alerts = await _unitOfWork.PriceAlerts.GetByUserIdAsync(userId);
        var alertDtos = new List<PriceAlertDto>();

        foreach (var alert in alerts)
        {
            var dto = _mapper.Map<PriceAlertDto>(alert);
            dto.CurrentLowestPrice = await _priceComparisonService.GetLowestPriceAsync(alert.ProductId);
            alertDtos.Add(dto);
        }

        return alertDtos;
    }

    public async Task<PriceAlertDto?> GetByIdAsync(Guid id)
    {
        var alert = await _unitOfWork.PriceAlerts.GetByIdAsync(id);
        if (alert == null) return null;

        var dto = _mapper.Map<PriceAlertDto>(alert);
        dto.CurrentLowestPrice = await _priceComparisonService.GetLowestPriceAsync(alert.ProductId);
        return dto;
    }

    public async Task<PriceAlertDto> CreateAsync(Guid userId, CreateAlertDto dto)
    {
        // Check if alert already exists
        var existingAlert = await _unitOfWork.PriceAlerts.GetByUserAndProductAsync(userId, dto.ProductId);
        if (existingAlert != null)
        {
            existingAlert.TargetPrice = dto.TargetPrice;
            existingAlert.IsActive = true;
            existingAlert.IsTriggered = false;
            existingAlert.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PriceAlerts.Update(existingAlert);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PriceAlertDto>(existingAlert);
        }

        var alert = new PriceAlert
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = dto.ProductId,
            TargetPrice = dto.TargetPrice,
            IsActive = true,
            IsTriggered = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.PriceAlerts.AddAsync(alert);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PriceAlertDto>(alert);
    }

    public async Task<PriceAlertDto?> UpdateAsync(Guid id, UpdateAlertDto dto)
    {
        var alert = await _unitOfWork.PriceAlerts.GetByIdAsync(id);
        if (alert == null) return null;

        if (dto.TargetPrice.HasValue) alert.TargetPrice = dto.TargetPrice.Value;
        if (dto.IsActive.HasValue) alert.IsActive = dto.IsActive.Value;

        alert.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.PriceAlerts.Update(alert);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PriceAlertDto>(alert);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var alert = await _unitOfWork.PriceAlerts.GetByIdAsync(id);
        if (alert == null) return false;

        _unitOfWork.PriceAlerts.Remove(alert);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task CheckAndTriggerAlertsAsync()
    {
        var activeAlerts = await _unitOfWork.PriceAlerts.GetActiveAlertsAsync();

        foreach (var alert in activeAlerts)
        {
            var lowestPrice = await _priceComparisonService.GetLowestPriceAsync(alert.ProductId);

            if (lowestPrice.HasValue && lowestPrice.Value <= alert.TargetPrice)
            {
                await _notificationService.SendPriceAlertEmailAsync(alert, lowestPrice.Value);

                alert.IsTriggered = true;
                alert.TriggeredAt = DateTime.UtcNow;
                alert.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.PriceAlerts.Update(alert);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
