namespace SkillHub.API.Entities;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime ModifiedAt { get; set; }
}

public class BaseUser : IAuditableEntity
{
    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

public class Freelancer : BaseUser
{
    public string Bio { get; set; } = null!;

    public string Title { get; set; } = null!;
    
    public string ProfilePhotoId { get; set; } = null!;

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Project> Projects { get; } = new List<Project>();

    public void UpdateProfile(string firstName, string lastName, string bio, string profilePhotoId, string title, List<Skill> skills)
    {
        Skills = skills;
        FirstName = firstName;
        LastName = lastName;
        Title = title;
        Bio = bio;
        ProfilePhotoId = profilePhotoId;
    }
}
// TODO add work experience table, maybe education also
public class Client : BaseUser
{
    public string WebsiteUrl { get; set; } = null!;
    
    public string CompanyName { get; set; } = null!;
    
    public string ClientInfo { get; set; } = string.Empty;
    
    public void UpdateProfile(string firstName, string lastName, string websiteUrl, string companyName, string clientInfo)
    {
        FirstName = firstName;
        LastName = lastName;
        WebsiteUrl = websiteUrl;
        CompanyName = companyName;
        ClientInfo = clientInfo;
    }
    
    public void AddProject(string title, string description, decimal budget, ExperienceLevel experienceLevel, ICollection<Skill> skills)
    {
        var project = new Project(title, description, budget, UserId, skills)
        {
            ExperienceLevel = experienceLevel
        };
        Projects.Add(project);
    }

    public ICollection<Project> Projects { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = null!;
}

public class Project
{
    public Project(string title, string description, decimal budget, string clientId, ICollection<Skill> skills)
    {
        Title = title;
        Description = description;
        Budget = budget;
        ClientId = clientId;
        Skills = skills;
    }

    public int Id { get; set; }
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

public enum ProjectStatus
{
    AcceptingProposals,
    InProgress,
    Completed,
    Aborted
}

public enum ExperienceLevel
{
    Entry,
    Intermediate,
    Expert
}

public enum ProposalStatus
{
    Pending,
    Accepted,
    Rejected
}

public class Proposal : IAuditableEntity
{
    public Proposal(string freelancerId, int projectId, string coverLetter)
    {
        ProjectId = projectId;
        CoverLetter = coverLetter;
        FreelancerId = freelancerId;
    }

    public int ProposalId { get; set; }
    public string FreelancerId { get; set; }
    public int ProjectId { get; set; }
    public string CoverLetter { get; set; }
    public ProposalStatus Status { get; set; } = ProposalStatus.Pending;
    public Project Project { get; set; } = null!;
    public Freelancer Freelancer { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

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

public class FreelancerSkill
{
    public FreelancerSkill(int freelancerId, int skillId)
    {
        FreelancerId = freelancerId;
        SkillId = skillId;
    }

    public int FreelancerId { get; set; }
    public int SkillId { get; set; }
}

public class ProjectSkill
{
    public ProjectSkill(int freelancerId, int skillId)
    {
        FreelancerId = freelancerId;
        SkillId = skillId;
    }

    public int FreelancerId { get; set; }
    public int SkillId { get; set; }
}

public class Review : IAuditableEntity
{
    public Review(int projectId, double rating, string reviewText)
    {
        ProjectId = projectId;
        Rating = rating;
        ReviewText = reviewText;
    }

    public int Id { get; set; }
    public int ProjectId { get; set; }
    public double Rating { get; set; }
    public string ReviewText { get; set; }
    public Project Project { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public void UpdateReview(double rating, string reviewText)
    {
        Rating = rating;
        ReviewText = reviewText;
    }
}