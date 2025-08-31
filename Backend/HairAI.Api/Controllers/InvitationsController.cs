using HairAI.Application.Features.Invitations.Commands.AcceptInvitation;
using HairAI.Application.Features.Invitations.Commands.CreateInvitation;
using HairAI.Application.Features.Invitations.Queries.GetInvitationByToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairAI.Api.Controllers;

public class InvitationsController : BaseController
{
    [HttpGet("{token}")]
    public async Task<ActionResult<GetInvitationByTokenQueryResponse>> GetByToken(string token)
    {
        var query = new GetInvitationByTokenQuery { Token = token };
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CreateInvitationCommandResponse>> Create(CreateInvitationCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("accept")]
    [Authorize]
    public async Task<ActionResult<AcceptInvitationCommandResponse>> Accept(AcceptInvitationCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}