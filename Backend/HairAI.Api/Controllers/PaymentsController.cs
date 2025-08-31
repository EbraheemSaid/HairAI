using HairAI.Application.Features.Payments.Commands.HandlePaymentWebhook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairAI.Api.Controllers;

[Authorize]
public class PaymentsController : BaseController
{
    [HttpPost("webhook")]
    public async Task<ActionResult<HandlePaymentWebhookCommandResponse>> HandleWebhook(HandlePaymentWebhookCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}