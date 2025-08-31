using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;

namespace HairAI.Application.Features.Payments.Commands.HandlePaymentWebhook;

public class HandlePaymentWebhookCommandHandler : IRequestHandler<HandlePaymentWebhookCommand, HandlePaymentWebhookCommandResponse>
{
    private readonly IApplicationDbContext _context;

    public HandlePaymentWebhookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlePaymentWebhookCommandResponse> Handle(HandlePaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment
        {
            SubscriptionId = request.SubscriptionId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = request.Status,
            PaymentGatewayReference = request.PaymentGatewayReference,
            ProcessedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        return new HandlePaymentWebhookCommandResponse
        {
            Success = true,
            Message = "Payment recorded successfully",
            PaymentId = payment.Id
        };
    }
}