using SkillHub.API.Entities;
using SkillHub.API.Services;

namespace SkillHub.API.Features.FreelancerProfile.Commands;

public class UpdateProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/freelancer/profile/",
                (IMediator mediator, ClaimsPrincipal claimsPrincipal, Command request, CancellationToken cancellationToken) =>
                {
                    request.Claims = claimsPrincipal.Claims;
                    return mediator.Send(request, cancellationToken);
                })
            .WithName(nameof(UpdateProfile))
            .WithTags(nameof(Command))
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policy.Freelancer);
    }

    public class Command : IRequest<Result>
    {
        internal IEnumerable<Claim> Claims { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public string ProfilePhotoId { get; set; } = null!;
        public int[] SkillIds { get; set; } = null!;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.FirstName)
                .NotEmpty();
            RuleFor(p => p.LastName)
                .NotEmpty();
            RuleFor(p => p.Title)
                .NotEmpty();
            RuleFor(p => p.Bio)
                .MinimumLength(20)
                .MaximumLength(2000);
            RuleFor(p => p.ProfilePhotoId)
                .NotEmpty();
            RuleFor(p => p.SkillIds)
                .NotEmpty();
        }
    }
    
    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApiDbContext _context;
        private readonly ISkillsService _skillsService;

        public Handler(ApiDbContext context, ISkillsService skillsService)
        {
            _context = context;
            _skillsService = skillsService;
        }
        
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken = default)
        {
            var user = request.Claims.GetUserInfo();
            var freelancer = await _context.Freelancers
                                 .FirstOrDefaultAsync(f => f.UserId == user.Id, cancellationToken)
                             ?? new Freelancer(user.Id, user.UserName, user.Email);

            var skillsResult = await _skillsService.GetSkillsFromIds(request.SkillIds, cancellationToken);
            if (!skillsResult.IsSuccess)
                return skillsResult;
            
            freelancer.UpdateProfile(request.FirstName, request.LastName, request.Bio, request.ProfilePhotoId, request.Title, skillsResult.Value!);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}