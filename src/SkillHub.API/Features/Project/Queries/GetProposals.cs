using AutoMapper;
using SkillHub.API.Features.Proposal.Queries;

namespace SkillHub.API.Features.Project.Queries;

public class GetProposals : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/project/{projectId:int}/proposals",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal,
                        CancellationToken cancellationToken, int projectId) =>
                    mediator.Send(new Command(claimsPrincipal.GetUser(), projectId), cancellationToken))
            .WithName(nameof(GetProposals))
            .WithTags(nameof(Proposal))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result<List<ProposalDto>>>
    {
        public Command(User user, int projectId)
        {
            User = user;
            ProjectId = projectId;
        }

        public User User { get; }
        public int ProjectId { get; }
    }
    
    public class Handler : IRequestHandler<Command, Result<List<ProposalDto>>>
    {
        private readonly ApiDbContext _apiDbContext;
        private readonly IMapper _mapper;

        public Handler(ApiDbContext apiDbContext, IMapper mapper)
        {
            _apiDbContext = apiDbContext;
            _mapper = mapper;
        }
        
        public async Task<Result<List<ProposalDto>>> Handle(Command request, CancellationToken cancellationToken)
        {
            var project =
                await _apiDbContext.Projects
                    .Include(x => x.Proposals)
                    .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
                return DomainErrors.ProjectNotFound;

            if (project.ClientId != request.User.Id)
                return DomainErrors.ClientNotAuthorized;

            var proposals = project.Proposals.ToList();

            return _mapper.Map<List<ProposalDto>>(proposals);
        }
    }
}