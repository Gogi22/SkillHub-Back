namespace SkillHub.API;

public static class DomainErrors
{
    public static readonly Error ClientNotFound = new("ClientNotFound", "Client with the provided Id does not exist.");

    public static readonly Error FreelancerNotFound =
        new("FreelancerNotFound", "Freelancer with the provided Id does not exist.");

    public static readonly Error ClientNotAuthorized =
        new("ClientNotAuthorized", "Client is not authorized to perform this action.");

    public static class Review
    {
        public static readonly Error ProjectNotFound =
            new("Review.ProjectNotFound", "Project with the provided Id does not exist.");

        public static readonly Error ProjectNotCompleted =
            new("Review.ProjectNotCompleted", "Can not write a review for uncompleted Project.");

        public static readonly Error ProjectAlreadyReviewed =
            new("Review.ProjectAlreadyReviewed", "Project has already been reviewed.");

        public static readonly Error ReviewNotFound =
            new("Review.ReviewNotFound", "Review with the provided Id does not exist.");
    }
}