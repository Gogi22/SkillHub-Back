using SkillHub.API.Entities;

namespace SkillHub.API.Services;

public interface IUserService
{
    public Task<Client?> GetClient(string userId, CancellationToken cancellationToken = default);
    public Task<Freelancer?> GetFreelancer(string userId, CancellationToken cancellationToken = default);
}