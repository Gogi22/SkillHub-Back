using SkillHub.API.Entities;
using SkillHub.API.Models;

namespace SkillHub.API.Services;

public class UserService : IUserService
{
    private readonly ApiDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly string _identityApiUrl;

    public UserService(ApiDbContext dbContext, HttpClient httpClient, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _identityApiUrl = configuration["IdentityApiUrl"];
    }

    public async Task<Client?> GetClientAsync(string userId, CancellationToken cancellationToken = default)
    {
        var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (client is not null)
        {
            return client;
        }

        return await GetUserFromIdentity(userId, cancellationToken) as Client;
    }

    public async Task<Freelancer?> GetFreelancerAsync(string userId, CancellationToken cancellationToken = default)
    {
        var freelancer = await _dbContext.Freelancers.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (freelancer is not null)
        {
            return freelancer;
        }

        return await GetUserFromIdentity(userId, cancellationToken) as Freelancer;
    }

    private async Task<BaseUser?> GetUserFromIdentity(string userId, CancellationToken cancellationToken = default)
    {
        var httpResult = await _httpClient.GetAsync(_identityApiUrl + "api/clients/" + userId, cancellationToken);
        if (!httpResult.IsSuccessStatusCode)
            return null;

        var result =
            await httpResult.Content.ReadFromJsonAsync<Result<GetUserResponse>>(
                cancellationToken: cancellationToken);
        if (result is null || !result.IsSuccess || result.Value is null)
            return null;

        var user = result.Value!;
        var baseUser = new BaseUser
        {
            UserId = user.UserId,
            Email = user.Email,
            UserName = user.UserName
        };

        try
        {
            if (user.Role == Role.Freelancer)
            {
                await _dbContext.Freelancers.AddAsync((baseUser as Freelancer)!, cancellationToken);
            }
            else // Client
            {
                await _dbContext.Clients.AddAsync((baseUser as Client)!, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return baseUser;
        }
        // catch the exception of user already being in the database otherwise just return null;
        catch (Exception) // catch the exception of user already being in the database
        {
            if (user.Role == Role.Freelancer)
            {
                return await _dbContext.Freelancers.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
            }

            // Client
            return await _dbContext.Clients.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        }
    }
}

// TODO make registered-users queue persistent