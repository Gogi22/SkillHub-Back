namespace SkillHub.API.Entities;

public class Review : BaseEntity<int>
{
    public Review(int projectId, double rating, string reviewText)
    {
        ProjectId = projectId;
        Rating = rating;
        ReviewText = reviewText;
    }
    public int ProjectId { get; set; }
    public double Rating { get; set; }
    public string ReviewText { get; set; }
    public Project Project { get; set; } = null!;

    public void UpdateReview(double rating, string reviewText)
    {
        Rating = rating;
        ReviewText = reviewText;
    }
}