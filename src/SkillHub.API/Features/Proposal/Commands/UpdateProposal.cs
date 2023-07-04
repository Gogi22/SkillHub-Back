using SkillHub.API.Entities;

namespace SkillHub.API.Features.Proposal.Commands;

public class UpdateProposal : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/proposal/{proposalId:int}",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, int proposalId, Command request,
                    CancellationToken cancellationToken) =>
                {
                    request.User = claimsPrincipal.GetUser();
                    request.ProposalId = proposalId;
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(SubmitProposal))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Freelancer);
    }

    public class Command : IRequest<Result>
    {
        internal User User { get; set; } = null!;
        internal int ProposalId { get; set; }
        public string CoverLetter { get; set; } = null!;
    }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.CoverLetter)
                .MinimumLength(30);
        }
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
                    .FirstOrDefaultAsync(x => x.ProposalId == request.ProposalId, cancellationToken);
            
            if (proposal is null)
                return DomainErrors.Proposal.ProposalNotFound;
            
            if (proposal.FreelancerId != request.User.Id)
                return DomainErrors.FreelancerNotAuthorized;

            if (proposal.Status != ProposalStatus.Pending)
                return DomainErrors.Proposal.ProposalIsNotActive;

            proposal.CoverLetter = request.CoverLetter;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}