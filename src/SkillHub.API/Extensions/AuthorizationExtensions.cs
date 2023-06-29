using System.Security.Claims;
using Common;
using SkillHub.API.Common;

namespace SkillHub.API.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.Freelancer, policy => policy.RequireClaim(ClaimTypes.Role, Role.Freelancer.ToString()).RequireAuthenticatedUser());
            options.AddPolicy(Policy.Client, policy => policy.RequireClaim(ClaimTypes.Role, Role.Client.ToString()).RequireAuthenticatedUser());
        });

        return services;
    }
}