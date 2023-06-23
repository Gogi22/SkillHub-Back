using System.Net;
using Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace IdentityServer.Middleware;

public static class ExceptionExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature?.Error.GetType() == typeof(ValidationException))
                {
                    var error = contextFeature.Error as ValidationException;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var result = Result.Failure(error!.Errors.Select(x => new Error(x.PropertyName, x.ErrorMessage))
                        .ToArray());
                    await context.Response.WriteAsJsonAsync(result);
                    return;
                }

                if (env.IsProduction())
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsJsonAsync(Result.Failure(new Error("InternalServerError",
                        "Internal server error.")));
                }
            });
        });
    }
}