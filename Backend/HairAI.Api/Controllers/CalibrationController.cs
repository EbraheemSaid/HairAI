using HairAI.Application.Features.Calibration.Commands.CreateCalibrationProfile;
using HairAI.Application.Features.Calibration.Commands.DeactivateCalibrationProfile;
using HairAI.Application.Features.Calibration.Commands.UpdateCalibrationProfile;
using HairAI.Application.Features.Calibration.Queries.GetActiveProfilesForClinic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairAI.Api.Controllers;

[Authorize]
public class CalibrationController : BaseController
{
    [HttpGet("active")]
    public async Task<ActionResult<GetActiveProfilesForClinicQueryResponse>> GetActiveProfiles([FromQuery] Guid clinicId)
    {
        var query = new GetActiveProfilesForClinicQuery { ClinicId = clinicId };
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CreateCalibrationProfileCommandResponse>> Create(CreateCalibrationProfileCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateCalibrationProfileCommandResponse>> Update(Guid id, UpdateCalibrationProfileCommand command)
    {
        command.ProfileId = id;
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<DeactivateCalibrationProfileCommandResponse>> Deactivate(Guid id)
    {
        var command = new DeactivateCalibrationProfileCommand { ProfileId = id };
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}