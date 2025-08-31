using HairAI.Application.Features.Clinics.Commands.CreateClinic;
using HairAI.Application.Features.Clinics.Commands.UpdateClinic;
using HairAI.Application.Features.Clinics.Queries.GetAllClinics;
using HairAI.Application.Features.Clinics.Queries.GetClinicById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairAI.Api.Controllers;

[Authorize]
public class ClinicsController : BaseController
{
    [HttpGet]
    public async Task<ActionResult<GetAllClinicsQueryResponse>> GetAll()
    {
        var query = new GetAllClinicsQuery();
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetClinicByIdQueryResponse>> GetById(Guid id)
    {
        var query = new GetClinicByIdQuery { ClinicId = id };
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CreateClinicCommandResponse>> Create(CreateClinicCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateClinicCommandResponse>> Update(Guid id, UpdateClinicCommand command)
    {
        command.ClinicId = id;
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}