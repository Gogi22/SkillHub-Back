namespace SkillHub.API.Features.Review.Commands;

public class DeleteReview : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/review/{reviewId:int}",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, int reviewId,
                    CancellationToken cancellationToken) =>
                {
                    var request = new Command(claimsPrincipal.GetUser(), reviewId);
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(DeleteReview))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result>
    {
        public Command(User user, int reviewId)
        {
            User = user;
            ReviewId = reviewId;
        }

        internal User User { get; set; }
        public int ReviewId { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext _context;

        public Handler(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken = default)
        {
            var review = await _context.Reviews
                .Include(p => p.Project)
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

            if (review == null)
                return DomainErrors.Review.ReviewNotFound;

            if (review.Project.ClientId != request.User.Id)
                return DomainErrors.ClientNotAuthorized;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}