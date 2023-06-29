namespace SkillHub.API.Entities;

public class Freelancer
{
    
    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string? Bio { get; set; }
    
    public string? ProfileImageId { get; set; }

    public ICollection<UserSkill> UserSkills { get; set; } = null!;
    
    public ICollection<Review> Reviews { get; set; } = null!;
    // add a method that adds a review to the user's reviews collection
    
    public void AddReview(string reviewerId, double rating, string reviewText)
    {
        // check if the user has actually worked with the reviewer
        if (Reviews.Any(r => r.UserId == reviewerId))
        {
            throw new Exception("You have already reviewed this user");
        }   
    }
}

public class Client
{
    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
}

public class Project
{
    public Project(string title, string description, ProjectStatus projectStatus, decimal budget, string creatorId)
    {
        Title = title;
        Description = description;
        ProjectStatus = projectStatus;
        Budget = budget;
        CreatorId = creatorId;
    }

    public int ProjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Budget { get; set; }
    public ProjectStatus ProjectStatus { get; set; }
    public string CreatorId { get; set; }
    
    public Freelancer Creator { get; set; } = null!;

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
    public Proposal(int proposalId, string freelancerId, int projectId, string userId, string description, DateTime createdAt)
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
        SkillId = skillId;
        Name = name;
    }

    public int SkillId { get; set; }
    public string Name { get; set; }

}

public class UserSkill
{
    public UserSkill(int userId, int skillId)
    {
        UserId = userId;
        SkillId = skillId;
    }

    public int UserId { get; set; }
    public int SkillId { get; set; }

    public Freelancer Freelancer { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

public class Review
{
    public Review(int reviewId, string userId, int projectId, double rating, string reviewText, DateTime createdAt, Project project)
    {
        ReviewId = reviewId;
        UserId = userId;
        ProjectId = projectId;
        Rating = rating;
        ReviewText = reviewText;
        CreatedAt = createdAt;
        Project = project;
    }

    public int ReviewId { get; set; }
    public string UserId { get; set; }
    public int ProjectId { get; set; }
    public double Rating { get; set; }
    public string ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }

    public Freelancer Freelancer { get; set; } = null!;
    public Project Project { get; set; }
}

