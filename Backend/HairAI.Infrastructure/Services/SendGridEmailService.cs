using HairAI.Application.Common.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;

namespace HairAI.Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly string _apiKey;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(string apiKey, ILogger<SendGridEmailService> logger)
    {
        _apiKey = apiKey;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            _logger.LogInformation("Sending email to {Recipient} with subject '{Subject}'", to, subject);
            
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("noreply@hairai.com", "HairAI");
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);
            
            var response = await client.SendEmailAsync(msg);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation("Email sent successfully to {Recipient}", to);
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogWarning("SendGrid response for email to {Recipient}: Status {StatusCode}, Body {Body}", 
                    to, response.StatusCode, responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient} with subject '{Subject}'", to, subject);
            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }

    public async Task SendInvitationEmailAsync(string to, string invitationLink)
    {
        try
        {
            _logger.LogInformation("Sending invitation email to {Recipient}", to);
            
            var subject = "You've been invited to HairAI";
            var htmlContent = $"<p>You've been invited to join HairAI.</p><p>Click the link below to accept:</p><p><a href=\"{invitationLink}\">Accept Invitation</a></p>";
            var textContent = $"You've been invited to join HairAI.\n\nClick the link below to accept:\n{invitationLink}";
            
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("noreply@hairai.com", "HairAI");
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, textContent, htmlContent);
            
            var response = await client.SendEmailAsync(msg);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation("Invitation email sent successfully to {Recipient}", to);
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogWarning("SendGrid response for invitation email to {Recipient}: Status {StatusCode}, Body {Body}", 
                    to, response.StatusCode, responseBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send invitation email to {Recipient}", to);
            throw new InvalidOperationException($"Failed to send invitation email: {ex.Message}", ex);
        }
    }
}