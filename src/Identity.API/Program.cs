using Common.Extensions;
using Common.Middleware;
using Identity.API.Attributes;
using Identity.API.Extensions;
using Identity.API.Features;
using Identity.API.Features.Auth;
using Identity.API.Infrastructure;
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.ConfigureExceptionHandler(app.Environment.IsProduction());

var inMemory = bool.Parse(builder.Configuration["UseInMemoryDatabase"]);
app.ApplyMigrations<UserDbContext>(inMemory);

app.MapPost("/auth/register",
    async ([FromServices] IMediator mediator, Register.Command model, CancellationToken cancellationToken) =>
        (await mediator.Send(model, cancellationToken)).ToActionResult());


app.MapPost("/auth/login",
    async ([FromServices] IMediator mediator, Login.Command model, CancellationToken cancellationToken) =>
        (await mediator.Send(model, cancellationToken)).ToActionResult());


app.MapGet("/user/{userId:guid}",
    [ServiceFilter(typeof(ValidateInternalServiceMiddleware))]
    async ([FromServices] IMediator mediator, [FromRoute] Guid userId, CancellationToken cancellationToken) =>
        (await mediator.Send(new GetUser.Query(userId.ToString()), cancellationToken)).ToActionResult()).ExcludeFromDescription();

await app.RunAsync();