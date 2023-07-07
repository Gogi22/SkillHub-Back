using SkillHub.API.Entities;

namespace SkillHub.API.Features.Project.Commands;

public class DeleteProject : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/project/{projectId:int}",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, int projectId,
                    CancellationToken cancellationToken) => 
                    mediator.Send(new Command(claimsPrincipal.GetUser(), projectId), cancellationToken))
            .WithName(nameof(DeleteProject))
            .WithTags(nameof(Project))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result>
    {
        public Command(User user, int projectId)
        {
            User = user;
            ProjectId = projectId;
        }

        internal User User { get; set; }
        internal int ProjectId { get; set; }
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
                .Include(p => p.Proposals)
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.Status == ProjectStatus.AcceptingProposals, cancellationToken);
            
            if (project is null)
                return DomainErrors.ProjectNotFound;
            
            if (project.ClientId != request.User.Id)
                return DomainErrors.ClientNotAuthorized;

            project.Abort();
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}