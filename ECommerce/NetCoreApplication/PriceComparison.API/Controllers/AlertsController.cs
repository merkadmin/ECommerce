using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparison.Application.DTOs;
using PriceComparison.Application.Interfaces;

namespace PriceComparison.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PriceAlertDto>>>> GetMyAlerts()
    {
        var userId = GetUserId();
        var alerts = await _alertService.GetUserAlertsAsync(userId);
        return Ok(ApiResponse<List<PriceAlertDto>>.SuccessResponse(alerts));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PriceAlertDto>>> GetAlert(Guid id)
    {
        var alert = await _alertService.GetByIdAsync(id);
        if (alert == null)
        {
            return NotFound(ApiResponse<PriceAlertDto>.FailResponse("Alert not found"));
        }

        return Ok(ApiResponse<PriceAlertDto>.SuccessResponse(alert));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PriceAlertDto>>> CreateAlert([FromBody] CreateAlertDto dto)
    {
        var userId = GetUserId();
        var alert = await _alertService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetAlert), new { id = alert.Id },
            ApiResponse<PriceAlertDto>.SuccessResponse(alert, "Alert created successfully"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PriceAlertDto>>> UpdateAlert(Guid id, [FromBody] UpdateAlertDto dto)
    {
        var alert = await _alertService.UpdateAsync(id, dto);
        if (alert == null)
        {
            return NotFound(ApiResponse<PriceAlertDto>.FailResponse("Alert not found"));
        }

        return Ok(ApiResponse<PriceAlertDto>.SuccessResponse(alert, "Alert updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteAlert(Guid id)
    {
        var result = await _alertService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse.FailResponse("Alert not found"));
        }

        return Ok(ApiResponse.SuccessResponse("Alert deleted successfully"));
    }
}
