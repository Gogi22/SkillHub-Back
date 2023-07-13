namespace SkillHub.API.Features.Review.Queries;

public class GetReviews : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/reviews/{freelancerId:guid}",
                (IMediator mediator, Guid freelancerId) =>
                    mediator.Send(new Command { UserId = freelancerId.ToString() }))
            .WithName(nameof(GetReviews))
            .WithTags(nameof(Review))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public class Command : IRequest<Result<List<Review>>>
    {
        public string UserId { get; set; } = default!;
    }

    public class Review
    {
        public Review(int reviewId, int projectId, double rating, string? reviewText, DateTime createdAt)
        {
            ReviewId = reviewId;
            ProjectId = projectId;
            Rating = rating;
            ReviewText = reviewText;
            CreatedAt = createdAt;
        }

        public int ReviewId { get; set; }
        public int ProjectId { get; set; }
        public double Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<List<Review>>>
    {
        private readonly ApiDbContext _context;

        public Handler(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Review>>> Handle(Command request, CancellationToken cancellationToken)
        {
            var freelancer =
                await _context.Freelancers
                    .AsNoTracking()
                    .Include(x => x.Projects)
                    .ThenInclude(x => x.Review)
                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (freelancer == null)
                return DomainErrors.FreelancerNotFound;

            var reviews = freelancer.Projects
                .Where(p => p.Review is not null)
                .Select(p => p.Review)
                .Select(r => new Review(r!.Id, r.ProjectId, r.Rating, r.ReviewText, r.CreatedAt))
                .ToList();

            return reviews;
        }
    }
}