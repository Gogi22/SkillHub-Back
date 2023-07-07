namespace SkillHub.API;

public static class DomainErrors
{
    public static readonly Error ClientNotFound = 
        new("ClientNotFound", "Client with the provided Id does not exist.");

    public static readonly Error FreelancerNotFound =
        new("FreelancerNotFound", "Freelancer with the provided Id does not exist.");

    public static readonly Error ClientNotAuthorized =
        new("ClientNotAuthorized", "Client is not authorized to perform this action.");

    public static readonly Error FreelancerNotAuthorized =
        new("FreelancerNotAuthorized", "Freelancer is not authorized to perform this action.");

    public static readonly Error ProjectNotFound =
        new("ProjectNotFound", "Project with the provided Id does not exist.");
    
    public static readonly Error ProjectNotInProgress = 
        new("ProjectNotInProgress", "Project is not in progress.");

    public static class Review
    {
        public static readonly Error ProjectNotCompleted =
            new("Review.ProjectNotCompleted", "Can not write a review for uncompleted Project.");

        public static readonly Error ProjectAlreadyReviewed =
            new("Review.ProjectAlreadyReviewed", "Project has already been reviewed.");

        public static readonly Error ReviewNotFound =
            new("Review.ReviewNotFound", "Review with the provided Id does not exist.");
    }

    public static class Skill
    {
        public static readonly Error SkillNotFound =
            new("Skill.SkillNotFound", "Skill with the provided Id does not exist.");
    }

    public static class Proposal
    {
        public static readonly Error ProjectNotAcceptingProposals =
            new("Proposal.ProjectNotAcceptingProposals", "Project is not accepting proposals.");

        public static readonly Error ProposalNotFound =
            new("Proposal.ProposalNotFound", "Proposal with the provided Id does not exist.");

        public static readonly Error ProposalIsNotActive =
            new("Proposal.ProposalIsNotActive", "Proposal is not active.");
    }
}