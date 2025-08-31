using MediatR;

namespace HairAI.Application.Features.Payments.Commands.HandlePaymentWebhook;

public class HandlePaymentWebhookCommand : IRequest<HandlePaymentWebhookCommandResponse>
{
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentGatewayReference { get; set; } = string.Empty;
}