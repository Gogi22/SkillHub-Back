namespace SkillHub.API.Features.FreelancerProfile.Queries;

public class GetProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/freelancer/profile/{freelancerId}",
                (IMediator mediator, string freelancerId, CancellationToken cancellationToken) =>
                {
                    var request = new Command(freelancerId);
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(GetProfile))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public class Command : IRequest<Result<Profile>>
    {
        public string FreelancerId { get; }

        public Command(string freelancerId)
        {
            FreelancerId = freelancerId;
        }
    }
    
    public class Profile
    {
        public Profile(string firstName, string lastName, string bio, string title, string profilePhotoUrl, List<string> skills)
        {
            FirstName = firstName;
            LastName = lastName;
            Bio = bio;
            Title = title;
            ProfilePhotoUrl = profilePhotoUrl;
            Skills = skills;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string Title { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public List<string> Skills { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, Result<Profile>>
    {
        private readonly ApiDbContext _context;

        public Handler(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Profile>> Handle(Command request, CancellationToken cancellationToken)
        {
            var freelancer = await _context.Freelancers
                .FirstOrDefaultAsync(c => c.UserId == request.FreelancerId, cancellationToken);

            if (freelancer is null)
                return DomainErrors.FreelancerNotFound;
            
            var url = "http://localhost:7007/profile-photos/" + freelancer.ProfilePhotoId; // TODO move this to configuration and different service class
            return new Profile(freelancer.FirstName, freelancer.LastName, freelancer.Bio, freelancer.Title, 
                url, freelancer.Skills.Select(s => s.Name).ToList());
        }
    }
}

