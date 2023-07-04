namespace SkillHub.API.Features.Project.Queries;

public class ProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Budget { get; set; }
    public string ProjectStatus { get; set; } = null!;
    public string ExperienceLevel { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string? FreelancerId { get; set; }
}