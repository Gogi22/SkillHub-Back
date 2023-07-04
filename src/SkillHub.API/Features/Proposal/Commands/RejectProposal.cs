using SkillHub.API.Entities;

namespace SkillHub.API.Features.Proposal.Commands;

public class RejectProposal : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/proposal/reject",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, Command request,
                    CancellationToken cancellationToken) =>
                {
                    request.User = claimsPrincipal.GetUser();
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(RejectProposal))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result>
    {
        internal User User { get; set; } = null!;
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
                .FirstOrDefaultAsync(x => x.ProposalId == request.ProposalId, cancellationToken);

            if (proposal is null)
                return DomainErrors.Proposal.ProposalNotFound;

            if (proposal.Project.ClientId != request.User.Id)
                return DomainErrors.ClientNotAuthorized;

            proposal.Status = ProposalStatus.Rejected;

            return Result.Success();
        }
    }
}