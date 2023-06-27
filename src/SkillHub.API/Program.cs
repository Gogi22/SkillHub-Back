using System.Security.Claims;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkillHub.API.Middleware;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// builder.Services.AddValidationSetup();

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddDbContext<UserDbContext>(options =>
//     options.UseInMemoryDatabase("MyDB"));
//
// builder.Services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(Login).Assembly); });
//
// builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// builder.Services.AddSingleton<JwtSettings>(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
//
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.CustomSchemaIds( type => type.ToString() );
//     options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
//     {
//         Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
//         In = ParameterLocation.Header,
//         Name = "Authorization",
//         Type = SecuritySchemeType.ApiKey
//     });
//     options.OperationFilter<SecurityRequirementsOperationFilter>();
// });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireClaim(ClaimTypes.Role, "admin").RequireAuthenticatedUser());
    options.AddPolicy("freelancer", policy => policy.RequireClaim(ClaimTypes.Role, "freelancer").RequireAuthenticatedUser());
    options.AddPolicy("client", policy => policy.RequireClaim(ClaimTypes.Role, "client").RequireAuthenticatedUser());
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.ConfigureExceptionHandler();    
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet("protected", () => "secret 2").RequireAuthorization("admin");;

app.Run();