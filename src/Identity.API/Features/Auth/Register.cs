using System.Globalization;
using System.Security.Claims;
using Common;
using Common.Options;
using FluentValidation;
using IdentityServer.Entities;
using IdentityServer.Events;
using IdentityServer.Extensions;
using IdentityServer.Helpers;
using IdentityServer.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Features.Auth;

public class Register
{
    private const string QueueName = "registered_users";
    public class Command : IRequest<Result<UserInfo>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.Password)
                .NotEmpty()
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
                .NotNull()
                .Must(role => string.Equals(role, "freelancer", StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(role, "client", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Role must be either 'freelancer' or 'Client'.");
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
                return Result.Failure<UserInfo>(DomainErrors.Register.UserAlreadyExists);
            }

            var (passwordHash, passwordSalt) = PasswordManager.CreatePasswordHash(request.Password);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.Role))
            };

            var user = new User(request.UserName, request.Email, passwordHash, passwordSalt,
                claims.Select(uc => new UserClaim(uc.Type, uc.Value)).ToList());

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var userRegisteredEvent = new UserRegisteredEvent(user);

            _eventProducer.Publish(userRegisteredEvent, QueueName);

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            var token = PasswordManager.GenerateToken(user.UserName, claims, _jwtSettings);

            return Result.Success(new UserInfo(user.UserName, claims.GetRole(), token));
        }
    }
}