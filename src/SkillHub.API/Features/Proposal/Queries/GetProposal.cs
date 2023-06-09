using AutoMapper;

namespace SkillHub.API.Features.Proposal.Queries;

public class GetProposal : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/proposal/{ProposalId:int}",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, int proposalId,
                        CancellationToken cancellationToken) =>
                    mediator.Send(new Command(claimsPrincipal.GetUser(), proposalId), cancellationToken))
            .WithName(nameof(GetProposal))
            .WithTags(nameof(Proposal))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result<ProposalDto>>
    {
        public Command(User user, int proposalId)
        {
            User = user;
            ProposalId = proposalId;
        }

        internal User User { get; set; }
        internal int ProposalId { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<ProposalDto>>
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public Handler(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<ProposalDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var proposal = await _context.Proposals
                .AsNoTracking()
                .Include(p => p.Project)
                .FirstOrDefaultAsync(p => p.Id == request.ProposalId, cancellationToken);

            if (proposal is null)
                return DomainErrors.Proposal.ProposalNotFound;

            if (proposal.Project.ClientId != request.User.Id)
                return DomainErrors.ClientNotAuthorized;

            return _mapper.Map<ProposalDto>(proposal);
        }
    }
}