namespace HairAI.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendInvitationEmailAsync(string to, string invitationLink);
}