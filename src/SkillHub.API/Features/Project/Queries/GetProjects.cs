using AutoMapper;
using SkillHub.API.Entities;

namespace SkillHub.API.Features.Project.Queries;

public class GetProjects : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/projects",
                (IMediator mediator) => mediator.Send(new Command()))
            .WithName(nameof(GetProjects))
            .WithTags(nameof(Project))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public class Command : IRequest<Result<List<ProjectDto>>>
    {
    }

    public class Handler : IRequestHandler<Command, Result<List<ProjectDto>>>
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public Handler(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<ProjectDto>>> Handle(Command request, CancellationToken cancellationToken)
        {
            var projects = await _context.Projects
                .AsNoTracking()
                .Include(p => p.Skills)
                .Include(p => p.Review)
                .Where(x => x.Status == ProjectStatus.AcceptingProposals)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ProjectDto>>(projects);
        }
    }
}