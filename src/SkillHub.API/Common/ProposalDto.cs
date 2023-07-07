using SkillHub.API.Entities;

namespace SkillHub.API.Common;

public class ProposalDto
{
    public string FreelancerId { get; set; } = null!;
    public int ProjectId { get; set; }
    public string CoverLetter { get; set; } = null!;
    public ProposalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}