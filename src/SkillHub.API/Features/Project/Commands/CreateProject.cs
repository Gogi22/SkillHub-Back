using SkillHub.API.Entities;
using SkillHub.API.Services;

namespace SkillHub.API.Features.Project.Commands;

public class CreateProject : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/project",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, Command request,
                    CancellationToken cancellationToken) =>
                {
                    request.User = claimsPrincipal.GetUser();
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(CreateProject))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result>
    {
        internal User User { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int[] SkillIds { get; set; } = null!;
        public ExperienceLevel ExperienceLevel { get; set; }
        public decimal Budget { get; set; }
        public string Description { get; set; } = null!;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.Title)
                .MinimumLength(20);
            RuleFor(p => p.SkillIds)
                .Must(x => x.Length >= 3);
            RuleFor(p => p.ExperienceLevel)
                .IsInEnum();
            RuleFor(p => p.Budget)
                .InclusiveBetween(1, 100000);
            RuleFor(p => p.Description)
                .MinimumLength(30);
        }
    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext _context;
        private readonly ISkillsService _skillsService;

        public Handler(ApiDbContext context, ISkillsService skillsService)
        {
            _context = context;
            _skillsService = skillsService;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == request.User.Id, cancellationToken);

            if (client is null)
                return DomainErrors.ClientNotFound;

            var skillsResult = await _skillsService.GetSkillsFromIds(request.SkillIds, cancellationToken);
            if (!skillsResult.IsSuccess)
                return skillsResult;

            client.AddProject(request.Title, request.Description, request.Budget, request.ExperienceLevel,
                skillsResult.Value!);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}