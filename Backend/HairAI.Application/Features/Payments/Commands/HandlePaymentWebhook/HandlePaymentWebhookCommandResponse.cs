using MediatR;

namespace HairAI.Application.Features.Payments.Commands.HandlePaymentWebhook;

public class HandlePaymentWebhookCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? PaymentId { get; set; }
}