using Common.Extensions;
using Common.Middleware;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCarter();
builder.Services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(Program).Assembly); });
builder.Services.AddFluentValidation(new[] { typeof(Program).Assembly });

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.ConfigureExceptionHandler(app.Environment.IsProduction());

app.UseAuthentication();

app.UseAuthorization();

app.MapCarter();

app.Run();