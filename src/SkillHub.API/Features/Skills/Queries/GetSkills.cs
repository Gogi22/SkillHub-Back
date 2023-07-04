namespace SkillHub.API.Features.Skills.Queries;

public class GetSkills : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/skills",
                (IMediator mediator, CancellationToken cancellationToken) =>
                    mediator.Send(new Command(), cancellationToken))
            .WithName(nameof(GetSkills))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public class Command : IRequest<Result<List<Skill>>>
    {
    }

    public class Skill
    {
        public Skill(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<List<Skill>>>
    {
        private readonly ApiDbContext _context;

        public Handler(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Skill>>> Handle(Command request, CancellationToken cancellationToken)
        {
            return await _context.Skills.AsNoTracking().Select(x => new Skill(x.SkillId, x.Name))
                .ToListAsync(cancellationToken);
        }
    }
} 