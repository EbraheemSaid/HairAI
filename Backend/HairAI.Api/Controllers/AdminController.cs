using HairAI.Application.Features.Admin.Commands.ManuallyActivateSubscription;
using HairAI.Application.Features.Admin.Commands.ManuallyCreateClinic;
using HairAI.Application.Features.Admin.Commands.ManuallyLogPayment;
using HairAI.Application.Features.Admin.Commands.CreateUser;
using HairAI.Application.Features.Admin.Commands.DeleteUser;
using HairAI.Application.Features.Admin.Commands.ToggleUserStatus;
using HairAI.Application.Features.Admin.Queries.GetAllUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairAI.Api.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class AdminController : BaseController
{
    [HttpPost("clinics")]
    public async Task<ActionResult<ManuallyCreateClinicCommandResponse>> CreateClinic(ManuallyCreateClinicCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("subscriptions")]
    public async Task<ActionResult<ManuallyActivateSubscriptionCommandResponse>> ActivateSubscription(ManuallyActivateSubscriptionCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("payments")]
    public async Task<ActionResult<ManuallyLogPaymentCommandResponse>> LogPayment(ManuallyLogPaymentCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    // User Management Endpoints
    [HttpGet("users")]
    public async Task<ActionResult<GetAllUsersQueryResponse>> GetAllUsers()
    {
        var query = new GetAllUsersQuery();
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPost("users")]
    public async Task<ActionResult<CreateUserCommandResponse>> CreateUser(CreateUserCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("users/{id}")]
    public async Task<ActionResult<DeleteUserCommandResponse>> DeleteUser(string id)
    {
        var command = new DeleteUserCommand { UserId = id };
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPatch("users/{id}/activate")]
    public async Task<ActionResult<ToggleUserStatusCommandResponse>> ActivateUser(string id)
    {
        var command = new ToggleUserStatusCommand { UserId = id, IsActivating = true };
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPatch("users/{id}/deactivate")]
    public async Task<ActionResult<ToggleUserStatusCommandResponse>> DeactivateUser(string id)
    {
        var command = new ToggleUserStatusCommand { UserId = id, IsActivating = false };
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}