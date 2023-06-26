using Common.Extensions;
using IdentityServer.Attributes;
using IdentityServer.Extensions;
using IdentityServer.Features;
using IdentityServer.Features.Auth;
using IdentityServer.Infrastructure;
using IdentityServer.Middleware;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(type => type.ToString()));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRabbitMqProducer(builder.Configuration);

builder.Services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(Register).Assembly); });
builder.Services.AddFluentValidation(new[] { typeof(Register.Validator).Assembly });

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddJwtSettings(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.ConfigureExceptionHandler(app.Environment);
app.ApplyMigrations<UserDbContext>();

app.MapPost("/auth/register",
    async ([FromServices] IMediator mediator, Register.Command model, CancellationToken cancellationToken) => 
        (await mediator.Send(model, cancellationToken)).ToActionResult());


app.MapPost("/auth/login",
    async ([FromServices] IMediator mediator, Login.Command model, CancellationToken cancellationToken) =>
        (await mediator.Send(model, cancellationToken)).ToActionResult());


app.MapGet("/user",
    [ServiceFilter(typeof(ValidateInternalServiceMiddleware))]
    async ([FromServices] IMediator mediator, [FromBody] GetUser.Query query, CancellationToken cancellationToken) =>
        (await mediator.Send(query, cancellationToken)).ToActionResult()).ExcludeFromDescription();

await app.RunAsync();