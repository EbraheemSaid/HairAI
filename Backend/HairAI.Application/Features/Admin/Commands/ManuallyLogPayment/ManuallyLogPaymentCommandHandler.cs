using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;

namespace HairAI.Application.Features.Admin.Commands.ManuallyLogPayment;

public class ManuallyLogPaymentCommandHandler : IRequestHandler<ManuallyLogPaymentCommand, ManuallyLogPaymentCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IClinicAuthorizationService _authorizationService;

    public ManuallyLogPaymentCommandHandler(IApplicationDbContext context, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<ManuallyLogPaymentCommandResponse> Handle(ManuallyLogPaymentCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Only SuperAdmin can manually log payments
        if (!await _authorizationService.IsSuperAdminAsync())
        {
            return new ManuallyLogPaymentCommandResponse
            {
                Success = false,
                Message = "Access denied. Only SuperAdmin can manually log payments.",
                Errors = new List<string> { "Insufficient permissions for payment operations" }
            };
        }

        var payment = new Payment
        {
            SubscriptionId = request.SubscriptionId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = "succeeded",
            PaymentGatewayReference = request.PaymentGatewayReference,
            ProcessedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        return new ManuallyLogPaymentCommandResponse
        {
            Success = true,
            Message = "Payment logged successfully",
            PaymentId = payment.Id
        };
    }
}