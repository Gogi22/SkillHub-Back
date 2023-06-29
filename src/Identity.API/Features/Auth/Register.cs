using System.Security.Claims;
using Common;
using Common.Events;
using Common.Options;
using FluentValidation;
using IdentityServer.Entities;
using IdentityServer.Extensions;
using IdentityServer.Helpers;
using IdentityServer.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Features.Auth;

public class Register
{
    private const string QueueName = "registered_users"; // TODO move to a config file
    public class Command : IRequest<Result<UserInfo>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public Role Role { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.Password)
                .NotEqual(p => p.Email)
                .NotEqual(p => p.UserName)
                .MinimumLength(8)
                .Matches("[A-Za-z]") // At least one letter
                .Matches("[^A-Za-z0-9]") // At least one symbol
                .Matches("[a-z]") // At least one lowercase letter
                .Matches("[A-Z]"); // At least one UpperCase letter
            RuleFor(p => p.UserName)
                .NotEmpty()
                .MinimumLength(6);
            RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(p => p.Role)
                .NotNull();
            RuleFor(p => p.Role)
                .Must(role => Enum.IsDefined(typeof(Role), role))
                .WithMessage("Role must be either Freelancer or Client");
        }
    }

    public class Handler : IRequestHandler<Command, Result<UserInfo>>
    {
        private readonly UserDbContext _context;
        private readonly IEventProducer _eventProducer;
        private readonly JwtSettings _jwtSettings;

        public Handler(UserDbContext context, JwtSettings jwtSettings, IEventProducer eventProducer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwtSettings = jwtSettings;
            _eventProducer = eventProducer;
        }

        public async Task<Result<UserInfo>> Handle(Command request, CancellationToken cancellationToken = default)
        {
            var userExists =
                await _context.Users.AnyAsync(u => u.UserName == request.UserName || u.Email == request.Email,
                    cancellationToken);
            
            if (userExists)
            {
                return DomainErrors.Register.UserAlreadyExists;
            }

            var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(request.Password);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, request.Role.ToString())
            };

            var user = new User(request.UserName, request.Email, passwordHash, passwordSalt,
                claims.Select(uc => new UserClaim(uc.Type, uc.Value.ToRole())).ToList());

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var userRegisteredEvent = new UserRegisteredEvent(user.Id, user.Email, user.UserName, request.Role);

            _eventProducer.Publish(userRegisteredEvent, QueueName);

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            var token = PasswordManager.GenerateToken(user.UserName, claims, _jwtSettings);

            return new UserInfo(user.UserName, claims.GetRole(), token);
        }
    }
}

// TODO change result error array to hashset,
// and for password validation return same error message
// After that commit move everything to Identity.API namespace;