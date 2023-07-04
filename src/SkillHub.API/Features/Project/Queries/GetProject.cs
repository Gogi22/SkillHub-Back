using AutoMapper;

namespace SkillHub.API.Features.Project.Queries;

public class GetProject
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/project/{projectId:int}",
                (IMediator mediator, int projectId) =>
                    mediator.Send(new Command(projectId)))
            .WithName(nameof(GetProject))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public class Command : IRequest<Result<ProjectDto>>
    {
        public int ProjectId { get; }

        public Command(int projectId)
        {
            ProjectId = projectId;
        }
    }

    public class Handler : IRequestHandler<Command, Result<ProjectDto>>
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public Handler(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<Result<ProjectDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .AsNoTracking()
                .Include(p => p.Skills)
                .Include(p => p.Review)
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (project is null)
                return DomainErrors.ProjectNotFound;

            return _mapper.Map<ProjectDto>(project);
        }
    }
}