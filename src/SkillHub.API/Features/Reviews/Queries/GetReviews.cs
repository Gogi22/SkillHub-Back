using Carter;
using Common;
using MediatR;
using SkillHub.API.Infrastructure;

namespace SkillHub.API.Features.Reviews.Queries;

public class GetReviews : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/reviews/{userId:guid}",
                (IMediator mediator, Guid userId) => mediator.Send(new Command { UserId = userId.ToString() }))
            .WithName(nameof(GetReviews))
            .WithTags(nameof(Command))
            .RequireAuthorization();
    }

    public class Command : IRequest<Result<List<Response>>>
    {
        public string UserId { get; set; } = default!;
    }

    public class Response
    {
        public int ReviewId { get; set; }
        public int ProjectId { get; set; }
        public double Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<List<Response>>>
    {
        private readonly ApiDbContext _context;

        public Handler(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Response>>> Handle(Command request, CancellationToken cancellationToken)
        {
            // var reviewer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            // var reviews = await _context.Reviews
            //     .Where(r => r.UserId == request.UserId)
            //     .Select(r => new Response
            //     {
            //         ReviewId = r.ReviewId,
            //         ProjectId = r.ProjectId,
            //         Rating = r.Rating,
            //         ReviewText = r.ReviewText,
            //         CreatedAt = r.CreatedAt
            //     }).ToListAsync(cancellationToken);
            //
            // return reviews;
            return new List<Response>();
        }
    }
}