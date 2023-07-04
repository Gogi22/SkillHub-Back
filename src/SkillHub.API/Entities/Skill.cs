namespace SkillHub.API.Entities;

public class Skill
{
    public Skill(int skillId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Skill name cannot be null or empty", nameof(name));
        }

        SkillId = skillId;
        Name = name;
    }

    public int SkillId { get; set; }
    public string Name { get; set; }
}