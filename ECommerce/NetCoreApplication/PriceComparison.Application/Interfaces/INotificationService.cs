using PriceComparison.Core.Entities;

namespace PriceComparison.Application.Interfaces;

public interface INotificationService
{
    Task SendPriceAlertEmailAsync(PriceAlert alert, decimal currentPrice);
    Task SendWelcomeEmailAsync(User user);
    Task SendPasswordResetEmailAsync(User user, string resetToken);
}
