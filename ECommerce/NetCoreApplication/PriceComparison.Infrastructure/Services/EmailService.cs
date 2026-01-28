using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PriceComparison.Application.Interfaces;
using PriceComparison.Core.Entities;

namespace PriceComparison.Infrastructure.Services;

public class EmailService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPriceAlertEmailAsync(PriceAlert alert, decimal currentPrice)
    {
        var subject = $"Price Alert: {alert.Product?.Name ?? "Product"} has dropped!";
        var body = $@"
            <h2>Price Alert Triggered!</h2>
            <p>Great news! The price for <strong>{alert.Product?.Name}</strong> has dropped to your target price.</p>
            <ul>
                <li>Your Target Price: ${alert.TargetPrice:F2}</li>
                <li>Current Lowest Price: ${currentPrice:F2}</li>
            </ul>
            <p>Don't miss out on this deal!</p>
        ";

        await SendEmailAsync(alert.User?.Email ?? string.Empty, subject, body);
    }

    public async Task SendWelcomeEmailAsync(User user)
    {
        var subject = "Welcome to Price Comparison!";
        var body = $@"
            <h2>Welcome, {user.FirstName}!</h2>
            <p>Thank you for joining Price Comparison.</p>
            <p>With our service, you can:</p>
            <ul>
                <li>Compare prices across multiple retailers</li>
                <li>Track price history for your favorite products</li>
                <li>Set up price alerts to never miss a deal</li>
                <li>Build your wishlist and shopping cart</li>
            </ul>
            <p>Start saving today!</p>
        ";

        await SendEmailAsync(user.Email, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(User user, string resetToken)
    {
        var subject = "Reset Your Password";
        var baseUrl = _configuration["App:BaseUrl"] ?? "https://localhost";
        var resetLink = $"{baseUrl}/reset-password?token={resetToken}";

        var body = $@"
            <h2>Password Reset Request</h2>
            <p>Hi {user.FirstName},</p>
            <p>We received a request to reset your password. Click the link below to reset it:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>If you didn't request this, please ignore this email.</p>
            <p>This link will expire in 24 hours.</p>
        ";

        await SendEmailAsync(user.Email, subject, body);
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        // In a real implementation, you would use an email service like SendGrid, AWS SES, etc.
        // For now, we'll just log the email
        _logger.LogInformation("Sending email to {To}: {Subject}", to, subject);

        // Simulate async operation
        await Task.CompletedTask;

        // Example implementation with SMTP:
        /*
        using var client = new SmtpClient(_configuration["Smtp:Host"])
        {
            Port = int.Parse(_configuration["Smtp:Port"] ?? "587"),
            Credentials = new NetworkCredential(
                _configuration["Smtp:Username"],
                _configuration["Smtp:Password"]),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(_configuration["Smtp:From"] ?? "noreply@pricecomparison.com"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(to);

        await client.SendMailAsync(message);
        */
    }
}
