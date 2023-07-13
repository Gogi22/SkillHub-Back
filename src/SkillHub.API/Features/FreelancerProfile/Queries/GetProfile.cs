using AutoMapper;

namespace SkillHub.API.Features.FreelancerProfile.Queries;

public class GetProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/freelancer/{freelancerId}/profile",
                (IMediator mediator, string freelancerId, CancellationToken cancellationToken) =>
                {
                    var request = new Command(freelancerId);
                    return mediator.Send(request, cancellationToken);
                })
            .WithName("Freelancer.GetProfile")
            .WithTags(nameof(FreelancerProfile))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public class Command : IRequest<Result<Profile>>
    {
        public Command(string freelancerId)
        {
            FreelancerId = freelancerId;
        }

        public string FreelancerId { get; }
    }

    public class Profile
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string ProfilePhotoUrl { get; set; } = null!;
        public List<string> Skills { get; set; } = null!;
    }

    public class Handler : IRequestHandler<Command, Result<Profile>>
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public Handler(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<Profile>> Handle(Command request, CancellationToken cancellationToken)
        {
            var freelancer = await _context.Freelancers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.FreelancerId, cancellationToken);

            if (freelancer is null)
                return DomainErrors.FreelancerNotFound;

            freelancer.ProfilePhotoId = "http://localhost:7007/profile-photos/" + freelancer.ProfilePhotoId;
            // TODO move this to configuration and different service class
            return _mapper.Map<Profile>(freelancer);
        }
    }
}