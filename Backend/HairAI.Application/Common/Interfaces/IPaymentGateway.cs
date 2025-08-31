namespace HairAI.Application.Common.Interfaces;

public interface IPaymentGateway
{
    Task<string> CreateCheckoutSessionAsync(decimal amount, string currency, string successUrl, string cancelUrl);
    Task<bool> VerifyPaymentAsync(string paymentIntentId);
}