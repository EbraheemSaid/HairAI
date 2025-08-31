using HairAI.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace HairAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(ApiResponseFilter))]
public abstract class BaseController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>() 
        ?? throw new InvalidOperationException("IMediator not found in request services");
}