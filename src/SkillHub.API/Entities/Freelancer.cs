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

    public void UpdateProfile(string firstName, string lastName, string bio, string profilePhotoId, string title, IEnumerable<int> skillIds, List<Skill> skills)
    {
        Skills.Clear();
        foreach(var skillId in skillIds)
        {
            var skill = skills.FirstOrDefault(s => s.SkillId == skillId);
            if (skill != null) Skills.Add(skill);
        }
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

    public ICollection<Project> Projects { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = null!;
}

public class Project
{
    public Project(string title, string description, ProjectStatus projectStatus, decimal budget, string clientId,
        string? freelancerId)
    {
        Title = title;
        Description = description;
        ProjectStatus = projectStatus;
        Budget = budget;
        ClientId = clientId;
        FreelancerId = freelancerId;
    }

    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Budget { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
    public string ClientId { get; set; }
    public string? FreelancerId { get; set; }

    public Client Client { get; set; } = null!;
    public Freelancer Freelancer { get; set; } = null!;
    public Review? Review { get; set; }
}

public enum ProjectStatus
{
    NotStarted,
    InProgress,
    Completed,
    Aborted
}

public class Proposal
{
    public Proposal(int proposalId, string freelancerId, int projectId, string userId, string description,
        DateTime createdAt)
    {
        ProposalId = proposalId;
        ProjectId = projectId;
        UserId = userId;
        Description = description;
        CreatedAt = createdAt;
        FreelancerId = freelancerId;
    }

    public int ProposalId { get; set; }
    public string FreelancerId { get; set; }
    public int ProjectId { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public Freelancer Freelancer { get; set; } = null!;
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