namespace SkillHub.API.Entities;

public class Client : BaseUser
{
    public Client(string userId, string userName, string email) : base(userId, userName, email)
    {
    }

    public string WebsiteUrl { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string ClientInfo { get; set; } = string.Empty;

    public ICollection<Project> Projects { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = null!;

    public void UpdateProfile(string firstName, string lastName, string websiteUrl, string companyName,
        string clientInfo)
    {
        FirstName = firstName;
        LastName = lastName;
        WebsiteUrl = websiteUrl;
        CompanyName = companyName;
        ClientInfo = clientInfo;
    }

    public void AddProject(string title, string description, decimal budget, ExperienceLevel experienceLevel,
        ICollection<Skill> skills)
    {
        var project = new Project(title, description, budget, UserId, skills)
        {
            ExperienceLevel = experienceLevel
        };
        Projects.Add(project);
    }
}