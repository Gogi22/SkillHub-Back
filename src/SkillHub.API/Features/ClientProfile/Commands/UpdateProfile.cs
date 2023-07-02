using SkillHub.API.Services;

namespace SkillHub.API.Features.ClientProfile.Commands;

public class UpdateProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/client/profile/",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, Command request, CancellationToken cancellationToken) =>
                {
                    request.User = claimsPrincipal.GetUser();
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(UpdateProfile))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Client);
    }

    public class Command : IRequest<Result>
    {
        internal User User { get; set; } = null!;
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
                .Must(BeAValidUrl)
                .WithMessage("Please enter a valid URL.");
            RuleFor(p => p.FirstName)
                .NotEmpty();
            RuleFor(p => p.LastName)
                .NotEmpty();
            RuleFor(p => p.CompanyName)
                .NotEmpty();
        }
    }

    private static bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
    
    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext _context;
        private readonly IUserService _userService;

        public Handler(ApiDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken = default)
        {
            var client = await _userService.GetClientAsync(request.User.Id, cancellationToken);

            if (client is null)
                return DomainErrors.ClientNotFound;
            
            client.UpdateProfile(request.FirstName, request.LastName, request.WebsiteUrl, request.CompanyName, request.ClientInfo);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}