using MediatR;

namespace HairAI.Application.Features.Admin.Commands.ManuallyLogPayment;

public class ManuallyLogPaymentCommand : IRequest<ManuallyLogPaymentCommandResponse>
{
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentGatewayReference { get; set; } = string.Empty;
}