using Common.Extensions;
using Common.Middleware;
using MediatR.Extensions.FluentValidation.AspNetCore;
using SkillHub.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerSupport();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCarter();
builder.Services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(Program).Assembly); });
builder.Services.AddFluentValidation(new[] { typeof(Program).Assembly });

builder.Services.AddScoped<ISkillsService, SkillsService>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

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

var inMemory = bool.Parse(builder.Configuration["UseInMemoryDatabase"]);
app.ApplyMigrations<ApiDbContext>(inMemory);

app.UseAuthentication();

app.UseAuthorization();

app.MapCarter();

app.Run();