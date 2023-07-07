using SkillHub.API.Entities;

namespace SkillHub.API.Features.Proposal.Commands;

public class RejectProposal : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/proposal/{proposalId:int}/reject",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, int proposalId,
                    CancellationToken cancellationToken) 
                    => mediator.Send(new Command(claimsPrincipal.GetUser(), proposalId), cancellationToken))
            .WithName(nameof(RejectProposal))
            .WithTags(nameof(Proposal))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result>
    {
        public Command(User user, int proposalId)
        {
            User = user;
            ProposalId = proposalId;
        }

        internal User User { get; set; }
        public int ProposalId { get; set; }
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
            var proposal = await _context.Proposals
                .Include(x => x.Project)
                .FirstOrDefaultAsync(x => x.Id == request.ProposalId, cancellationToken);

            if (proposal is null)
                return DomainErrors.Proposal.ProposalNotFound;

            if (proposal.Project.ClientId != request.User.Id)
                return DomainErrors.ClientNotAuthorized;

            if (proposal.Status != ProposalStatus.Pending)
                return DomainErrors.Proposal.ProposalIsNotActive;

            proposal.Reject();

            return Result.Success();
        }
    }
}