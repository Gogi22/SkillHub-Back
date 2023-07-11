using Common.Extensions;
using Common.Middleware;
using Identity.API.Extensions;
using Identity.API.Features.Auth;
using Identity.API.Infrastructure;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(type => type.ToString()));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(Register).Assembly); });
builder.Services.AddFluentValidation(new[] { typeof(Register.Validator).Assembly });

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddJwtSettings(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder =>
        {
            policyBuilder
                .AllowAnyOrigin() 
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
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

await app.RunAsync();