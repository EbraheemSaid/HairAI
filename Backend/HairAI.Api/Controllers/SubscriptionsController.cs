using HairAI.Application.Features.Subscriptions.Commands.CreateSubscription;
using HairAI.Application.Features.Subscriptions.Commands.CancelSubscription;
using HairAI.Application.Features.Subscriptions.Queries.GetAllPlans;
using HairAI.Application.Features.Subscriptions.Queries.GetSubscriptionForClinic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairAI.Api.Controllers;

[Authorize]
public class SubscriptionsController : BaseController
{
    [HttpGet("plans")]
    public async Task<ActionResult<GetAllPlansQueryResponse>> GetAllPlans()
    {
        var query = new GetAllPlansQuery();
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("clinic/{id:guid}")]
    public async Task<ActionResult<GetSubscriptionForClinicQueryResponse>> GetForClinic(Guid id)
    {
        var query = new GetSubscriptionForClinicQuery { ClinicId = id };
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CreateSubscriptionCommandResponse>> Create(CreateSubscriptionCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<CancelSubscriptionCommandResponse>> Cancel(Guid id)
    {
        var command = new CancelSubscriptionCommand { SubscriptionId = id };
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}