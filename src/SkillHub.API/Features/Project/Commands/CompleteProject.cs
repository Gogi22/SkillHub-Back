using SkillHub.API.Entities;

namespace SkillHub.API.Features.Project.Commands;

public class CompleteProject : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/project/{projectId:int}/complete",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, int projectId,
                        CancellationToken cancellationToken) =>
                    mediator.Send(new Command(claimsPrincipal.GetUser(), projectId), cancellationToken))
            .WithName(nameof(CompleteProject))
            .WithTags(nameof(Project))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public record Command(User User, int ProjectId) : IRequest<Result>;

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
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
                return DomainErrors.ProjectNotFound;

            if (project.ClientId != request.User.Id)
                return DomainErrors.ClientNotAuthorized;

            if (project.Status != ProjectStatus.InProgress)
                return DomainErrors.ProjectNotInProgress;

            project.Complete();
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}