namespace SkillHub.API.Entities;

public class Project : BaseEntity<int>
{
    public Project(string title, string description, decimal budget, string clientId, ICollection<Skill> skills)
    {
        Title = title;
        Description = description;
        Budget = budget;
        ClientId = clientId;
        Skills = skills;
    }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Budget { get; set; }
    public ProjectStatus Status { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; }
    public string ClientId { get; set; }
    public string? FreelancerId { get; set; }

    public Client Client { get; set; } = null!;
    public Freelancer? Freelancer { get; set; }
    public Review? Review { get; set; }
    public ICollection<Proposal> Proposals { get; } = new List<Proposal>();
    public ICollection<Skill> Skills { get; set; }

    public void AddProposal(string freelancerId, string coverLetter)
    {
        var proposal = new Proposal(freelancerId, Id, coverLetter);
        Proposals.Add(proposal);
    }
}