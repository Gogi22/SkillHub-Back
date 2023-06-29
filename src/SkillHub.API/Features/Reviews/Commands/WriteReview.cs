using System.Security.Claims;
using Carter;
using Common;
using MediatR;
using SkillHub.API.Infrastructure;

namespace SkillHub.API.Features.Reviews.Commands;

public class WriteReview : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/review",
                (IMediator mediator, Command request, ClaimsPrincipal claimsPrincipal) =>
                {
                    var userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                                 ?? throw new Exception();
                    request.UserId = userId;
                    return mediator.Send(request);
                })
            .WithName(nameof(WriteReview))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    public class Command : IRequest<Result>
    {
        internal string UserId { get; set; } = null!;
        public int ProjectId { get; set; }
        public double Rating { get; set; }
        public string? ReviewText { get; set; }
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
            // add enw review if user actually worked on the project and hasn't already reviewed it.
            // var proposal = await _context.Projects
            return Result.Success();
        }
    }
}