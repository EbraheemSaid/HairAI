using HairAI.Application.Features.Patients.Commands.CreatePatient;
using HairAI.Application.Features.Patients.Commands.UpdatePatient;
using HairAI.Application.Features.Patients.Queries.GetPatientById;
using HairAI.Application.Features.Patients.Queries.GetPatientsForClinic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HairAI.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
public class PatientsController : BaseController
{
    [HttpGet]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<GetPatientsForClinicQueryResponse>> GetForClinic([FromQuery] Guid clinicId)
    {
        // SECURITY: Validate clinicId
        if (clinicId == Guid.Empty)
        {
            return BadRequest(new { 
                success = false, 
                message = "Invalid clinic ID" 
            });
        }
        
        var query = new GetPatientsForClinicQuery { ClinicId = clinicId };
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<GetPatientByIdQueryResponse>> GetById(Guid id)
    {
        // SECURITY: Validate id
        if (id == Guid.Empty)
        {
            return BadRequest(new { 
                success = false, 
                message = "Invalid patient ID" 
            });
        }
        
        var query = new GetPatientByIdQuery { PatientId = id };
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<CreatePatientCommandResponse>> Create(CreatePatientCommand command)
    {
        var response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [EnableRateLimiting("AuthPolicy")] // Add rate limiting for security
    public async Task<ActionResult<UpdatePatientCommandResponse>> Update(Guid id, UpdatePatientCommand command)
    {
        // SECURITY: Validate id
        if (id == Guid.Empty)
        {
            return BadRequest(new { 
                success = false, 
                message = "Invalid patient ID" 
            });
        }
        
        command.PatientId = id;
        var response = await Mediator.Send(command);
        return Ok(response);
    }
}