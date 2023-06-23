using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer.Attributes;

public class ValidateInternalServiceMiddleware : ActionFilterAttribute
{
    private readonly IConfiguration _configuration;

    public ValidateInternalServiceMiddleware(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var secret = _configuration["Microservices:Secret"];
        
        if (context.HttpContext.Request.Headers.TryGetValue("Secret", out var requestSecret))
        {
            if (requestSecret != secret)
            {
                context.Result = new ObjectResult(Result.Failure(DomainErrors.InternalRequest.SecretHeaderInvalid));
                return;
            }
        }
        else
        {
            context.Result =  new ObjectResult(Result.Failure(DomainErrors.InternalRequest.SecretHeaderMissing));
            return;
        }

        await next();
    }
}