using SkillHub.API.Entities;

namespace SkillHub.API.Services;

public interface IUserService
{
    public Task<Client?> GetClientAsync(string userId, CancellationToken cancellationToken = default);
    public Task<Freelancer?> GetFreelancerAsync(string userId, CancellationToken cancellationToken = default);
}