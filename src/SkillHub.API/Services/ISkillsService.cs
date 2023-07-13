using SkillHub.API.Entities;

namespace SkillHub.API.Services;

public interface ISkillsService
{
    public Task<Result<List<Skill>>> GetSkillsFromIds(IEnumerable<int> skillIds,
        CancellationToken cancellationToken = default);
}