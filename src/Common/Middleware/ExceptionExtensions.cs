using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Common.Middleware;

public static class ExceptionExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app, bool isProduction)
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

                if (isProduction)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsJsonAsync(Result.Failure(new Error("InternalServerError",
                        "Internal server error.")));
                }
                else
                {
                    throw contextFeature?.Error ?? new Exception("An error occurred.");
                }
            });
        });
    }
}