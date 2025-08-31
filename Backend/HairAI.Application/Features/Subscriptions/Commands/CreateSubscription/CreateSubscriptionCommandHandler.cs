using MediatR;
using HairAI.Application.Common.Interfaces;
using HairAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HairAI.Application.Features.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, CreateSubscriptionCommandResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IClinicAuthorizationService _authorizationService;

    public CreateSubscriptionCommandHandler(IApplicationDbContext context, IPaymentGateway paymentGateway, IClinicAuthorizationService authorizationService)
    {
        _context = context;
        _paymentGateway = paymentGateway;
        _authorizationService = authorizationService;
    }

    public async Task<CreateSubscriptionCommandResponse> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // SECURITY: Check if user can create subscription for this clinic
        if (!await _authorizationService.CanAccessClinicAsync(request.ClinicId))
        {
            return new CreateSubscriptionCommandResponse
            {
                Success = false,
                Message = "Access denied. You cannot create subscriptions for this clinic.",
                Errors = new List<string> { "Unauthorized clinic access" }
            };
        }

        // Check if clinic exists
        var clinic = await _context.Clinics.FindAsync(new object[] { request.ClinicId }, cancellationToken);
        if (clinic == null)
        {
            return new CreateSubscriptionCommandResponse
            {
                Success = false,
                Message = "Clinic not found",
                Errors = new List<string> { "Clinic not found" }
            };
        }

        // Check if clinic already has an active subscription
        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.ClinicId == request.ClinicId && s.Status == "active", cancellationToken);
        
        if (existingSubscription != null)
        {
            return new CreateSubscriptionCommandResponse
            {
                Success = false,
                Message = "Clinic already has an active subscription",
                Errors = new List<string> { "Active subscription already exists" }
            };
        }

        // Get the subscription plan
        var plan = await _context.SubscriptionPlans.FindAsync(new object[] { request.PlanId }, cancellationToken);
        if (plan == null)
        {
            return new CreateSubscriptionCommandResponse
            {
                Success = false,
                Message = "Subscription plan not found",
                Errors = new List<string> { "Invalid subscription plan" }
            };
        }

        try
        {
            // Create payment session with Paymob
            var paymentKey = await _paymentGateway.CreateCheckoutSessionAsync(
                plan.PriceMonthly,
                plan.Currency,
                "https://yourdomain.com/payment/success",
                "https://yourdomain.com/payment/cancel"
            );

            // Create subscription in pending status
            var subscription = new Subscription
            {
                ClinicId = request.ClinicId,
                PlanId = request.PlanId,
                Status = "pending", // Will be activated after successful payment
                CurrentPeriodStart = null, // Will be set after payment confirmation
                CurrentPeriodEnd = null
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateSubscriptionCommandResponse
            {
                Success = true,
                Message = "Subscription created successfully. Complete payment to activate.",
                SubscriptionId = subscription.Id,
                PaymentKey = paymentKey,
                RedirectUrl = $"https://accept.paymob.com/api/acceptance/iframes/IFRAME_ID?payment_token={paymentKey}"
            };
        }
        catch (Exception ex)
        {
            return new CreateSubscriptionCommandResponse
            {
                Success = false,
                Message = "Failed to create payment session",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}