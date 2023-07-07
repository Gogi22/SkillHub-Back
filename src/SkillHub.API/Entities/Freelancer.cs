namespace SkillHub.API.Entities;

public class Freelancer : BaseUser
{
    public Freelancer(string userId, string userName, string email) : base(userId, userName, email)
    {
    }
    
    private Freelancer() : base(){}

    public string Bio { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string ProfilePhotoId { get; set; } = null!;

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Project> Projects { get; } = new List<Project>();

    public void UpdateProfile(string firstName, string lastName, string bio, string profilePhotoId, string title,
        List<Skill> skills)
    {
        Skills = skills;
        FirstName = firstName;
        LastName = lastName;
        Title = title;
        Bio = bio;
        ProfilePhotoId = profilePhotoId;
    }
}