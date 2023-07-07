using SkillHub.API.Entities;

namespace SkillHub.API.Features.ClientProfile.Commands;

public class UpdateProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/client/profile/",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, Command request,
                    CancellationToken cancellationToken) =>
                {
                    request.Claims = claimsPrincipal.Claims;
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(UpdateProfile))
            .WithTags(nameof(ClientProfile))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    private static bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public class Command : IRequest<Result>
    {
        internal IEnumerable<Claim> Claims { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string WebsiteUrl { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string ClientInfo { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.WebsiteUrl)
                .NotEmpty()
                .MaximumLength(40)
                .Must(BeAValidUrl)
                .WithMessage("Please enter a valid URL.");
            RuleFor(p => p.FirstName)
                .NotEmpty()
                .MaximumLength(20);
            RuleFor(p => p.LastName)
                .MaximumLength(30)
                .NotEmpty();
            RuleFor(p => p.CompanyName)
                .MaximumLength(30)
                .NotEmpty();
            RuleFor(p => p.ClientInfo)
                .MaximumLength(500);
        }
    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext _context;

        public Handler(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken = default)
        {
            var user = request.Claims.GetUserInfo();
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == user.Id, cancellationToken);
            var newClient = client is null;
            client ??= new Client(user.Id, user.UserName, user.Email);
            
            client.UpdateProfile(request.FirstName, request.LastName, request.WebsiteUrl, request.CompanyName,
                request.ClientInfo);
            if (newClient)
            {
                await _context.Clients.AddAsync(client, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}