namespace SkillHub.API.Features.Reviews.Commands;

public class UpdateReview : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/review",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, Command request,
                    CancellationToken cancellationToken) =>
                {
                    request.User = claimsPrincipal.GetUser();
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(UpdateReview))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result>
    {
        internal User User { get; set; } = null!;
        public int ReviewId { get; set; }
        public double Rating { get; set; }
        public string ReviewText { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.ReviewId)
                .NotEmpty();
            RuleFor(v => v.Rating)
                .InclusiveBetween(1, 5);
            RuleFor(v => v.ReviewText)
                .MinimumLength(10)
                .MaximumLength(2000);
        }
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
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

            if (review is null)
                return DomainErrors.Reviews.ReviewNotFound;

            if (review.Project.ClientId != request.User.Id)
                return DomainErrors.ClientNotAuthorized;

            review.UpdateReview(request.Rating, request.ReviewText);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}