using Microsoft.OpenApi.Models;

namespace SkillHub.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerSupport(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
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
    }
}