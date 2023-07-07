namespace SkillHub.API.Entities;

public class Proposal : BaseEntity<int>
{
    public Proposal(string freelancerId, int projectId, string coverLetter)
    {
        ProjectId = projectId;
        CoverLetter = coverLetter;
        FreelancerId = freelancerId;
    }

    public string FreelancerId { get; set; }
    public int ProjectId { get; set; }
    public string CoverLetter { get; set; }
    public ProposalStatus Status { get; set; } = ProposalStatus.Pending;
    public Project Project { get; set; } = null!;
    public Freelancer Freelancer { get; set; } = null!;

    public void Accept()
    {
        Project.FreelancerId = FreelancerId;
        Project.Status = ProjectStatus.InProgress;
        Status = ProposalStatus.Accepted;
    }

    public void Reject()
    {
        Status = ProposalStatus.Rejected;
    }
}