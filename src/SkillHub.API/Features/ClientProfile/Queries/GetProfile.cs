using AutoMapper;

namespace SkillHub.API.Features.ClientProfile.Queries;

public class GetProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/client/{clientId}/profile",
                (IMediator mediator, string clientId, CancellationToken cancellationToken) =>
                {
                    var request = new Command(clientId);
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(GetProfile))
            .WithTags(nameof(ClientProfile))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public class Command : IRequest<Result<ProfileDto>>
    {
        public Command(string clientId)
        {
            ClientId = clientId;
        }

        public string ClientId { get; }
    }

    public class ProfileDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string WebsiteUrl { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string ClientInfo { get; set; } = null!;
    }

    public class Handler : IRequestHandler<Command, Result<ProfileDto>>
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public Handler(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<ProfileDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var client = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken);

            if (client is null)
                return DomainErrors.ClientNotFound;

            return _mapper.Map<ProfileDto>(client);
        }
    }
}