namespace SkillHub.API.Features.ClientProfile.Queries;

public class GetProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/client/profile/{clientId}",
                (IMediator mediator, string clientId, CancellationToken cancellationToken) =>
                {
                    var request = new Command(clientId);
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(GetProfile))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public class Command : IRequest<Result<Profile>>
    {
        public string ClientId { get; }

        public Command(string clientId)
        {
            ClientId = clientId;
        }
    }
    
    public class Profile
    {
        public Profile(string firstName, string lastName, string websiteUrl, string companyName, string clientInfo)
        {
            FirstName = firstName;
            LastName = lastName;
            WebsiteUrl = websiteUrl;
            CompanyName = companyName;
            ClientInfo = clientInfo;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string WebsiteUrl { get; set; }
        public string CompanyName { get; set; }
        public string ClientInfo { get; set; }
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
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == request.ClientId, cancellationToken);

            if (client is null)
                return DomainErrors.ClientNotFound;

            return new Profile(client.FirstName, client.LastName, client.WebsiteUrl, client.CompanyName,
                client.ClientInfo);
        }
    }
}

