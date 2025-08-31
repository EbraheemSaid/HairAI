using MediatR;

namespace HairAI.Application.Features.Admin.Commands.ManuallyLogPayment;

public class ManuallyLogPaymentCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Guid? PaymentId { get; set; }
}