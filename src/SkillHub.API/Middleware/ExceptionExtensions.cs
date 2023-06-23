using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace SkillHub.API.Middleware;

public static class ExceptionExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "text/html";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                
                await context.Response.WriteAsync("something went wrong");
            });
        });
    }
}