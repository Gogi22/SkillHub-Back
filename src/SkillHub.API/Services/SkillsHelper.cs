using SkillHub.API.Entities;

namespace SkillHub.API.Services;

public class SkillsService : ISkillsService
{
    private readonly ApiDbContext _context;

    public SkillsService(ApiDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<List<Skill>>> GetSkillsFromIds(IEnumerable<int> skillIds, CancellationToken cancellationToken = default)
    {
        var allSkills = await _context.Skills.ToListAsync(cancellationToken);
        var skills = new List<Skill>();
        foreach(var id in skillIds)
        {
            var skill = allSkills.FirstOrDefault(s => s.SkillId == id);
            if (skill is null)
                return DomainErrors.Skill.SkillNotFound;
            skills.Add(skill);
        }

        return skills;
    }
}