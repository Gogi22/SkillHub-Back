using SkillHub.API.Entities;

namespace SkillHub.API.Features.Proposal.Commands;

public class SubmitProposal : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/proposal",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, Command request,
                    CancellationToken cancellationToken) =>
                {
                    request.User = claimsPrincipal.GetUser();
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(SubmitProposal))
            .WithTags(nameof(Proposal))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Freelancer);
    }

    public class Command : IRequest<Result>
    {
        internal User User { get; set; } = null!;
        public int ProjectId { get; set; }
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
            var project = await _context.Projects
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
                return DomainErrors.ProjectNotFound;

            if (project.Status != ProjectStatus.AcceptingProposals)
                return DomainErrors.Proposal.ProjectNotAcceptingProposals;

            var freelancer =
                await _context.Freelancers.FirstOrDefaultAsync(x => x.Id == request.User.Id, cancellationToken);
            if (freelancer is null)
                return DomainErrors.FreelancerNotFound;

            project.AddProposal(request.User.Id, request.CoverLetter);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}