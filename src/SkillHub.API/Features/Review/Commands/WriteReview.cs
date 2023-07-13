using SkillHub.API.Entities;

namespace SkillHub.API.Features.Review.Commands;

public class WriteReview : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/review",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, Command request,
                    CancellationToken cancellationToken) =>
                {
                    request.User = claimsPrincipal.GetUser();
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(WriteReview))
            .WithTags(nameof(Review))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result>
    {
        internal User User { get; set; } = null!;
        public int ProjectId { get; set; }
        public double Rating { get; set; }
        public string ReviewText { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.ProjectId)
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

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .Include(p => p.Review)
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

            var validationResult = ValidateProjectAndUser(request, project);
            if (validationResult != Error.None)
            {
                return validationResult;
            }

            project!.Review = new Entities.Review(project.Id, request.Rating, request.ReviewText);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private static Error ValidateProjectAndUser(Command request, Entities.Project? project)
        {
            var validations = new List<(Func<bool> Condition, Error Error)>
            {
                (() => project is null, DomainErrors.ProjectNotFound),
                (() => project!.Status != ProjectStatus.Completed, DomainErrors.Review.ProjectNotCompleted),
                (() => project!.ClientId != request.User.Id, DomainErrors.ClientNotAuthorized),
                (() => project!.Review != null, DomainErrors.Review.ProjectAlreadyReviewed)
            };

            return validations.FirstOrDefault(validation => validation.Condition()).Error ?? Error.None;
        }
    }
}