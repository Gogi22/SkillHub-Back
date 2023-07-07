using SkillHub.API.Entities;

namespace SkillHub.API.Features.Proposal.Commands;

public class DeleteProposal : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/proposal/{proposalId:int}",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, int proposalId,
                        CancellationToken cancellationToken) =>
                    mediator.Send(new Command(claimsPrincipal.GetUser(), proposalId), cancellationToken))
            .WithName(nameof(DeleteProposal))
            .WithTags(nameof(Proposal))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Freelancer);
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

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken = default)
        {
            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(x => x.Id == request.ProposalId, cancellationToken);

            if (proposal is null)
                return DomainErrors.Proposal.ProposalNotFound;

            if (proposal.FreelancerId != request.User.Id)
                return DomainErrors.FreelancerNotAuthorized;

            if (proposal.Status != ProposalStatus.Pending)
                return DomainErrors.Proposal.ProposalIsNotActive;

            _context.Proposals.Remove(proposal);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}