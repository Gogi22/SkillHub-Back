using SkillHub.API.Entities;
using SkillHub.API.Services;

namespace SkillHub.API.Features.Project.Commands;

public class UpdateProject : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/project/{projectId:int}",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, int projectId, Command request,
                    CancellationToken cancellationToken) =>
                {
                    request.User = claimsPrincipal.GetUser();
                    request.ProjectId = projectId;
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(UpdateProject))
            .WithTags(nameof(Project))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result>
    {
        internal User User { get; set; } = null!;
        internal int ProjectId { get; set; }
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
            var project = await _context.Projects
                .Include(x => x.Skills)
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId 
                && x.Status == ProjectStatus.AcceptingProposals, cancellationToken);
            
            if (project is null)
                return DomainErrors.ProjectNotFound;

            var skillsResult = await _skillsService.GetSkillsFromIds(request.SkillIds, cancellationToken);
            if (!skillsResult.IsSuccess)
                return skillsResult;
            
            project.Update(request.Title, request.Description, request.Budget, request.ExperienceLevel,
                skillsResult.Value!);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}