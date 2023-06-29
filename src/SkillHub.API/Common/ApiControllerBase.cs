using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SkillHub.API.Common;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
}

// create a test feature to test this functionality