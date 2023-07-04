using System.Security.Claims;
using Common;
using Common.Options;
using FluentValidation;
using Identity.API.Extensions;
using Identity.API.Helpers;
using Identity.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Features.Auth;

public class Login
{
    public class Command : IRequest<Result<UserInfo>>
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command, Result<UserInfo>>
    {
        private readonly UserDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public Handler(UserDbContext context, JwtSettings jwtSettings)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        public async Task<Result<UserInfo>> Handle(Command request, CancellationToken cancellationToken = default)
        {
            var users = await _context.Users.Include(u => u.Claims).ToListAsync(cancellationToken);
            var user = users.FirstOrDefault(x => x.UserName == request.UserName) ??
                       users.FirstOrDefault(x => x.Email == request.UserName);
            if (user == null ||
                !PasswordManager.IsValidPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return DomainErrors.Login.UserDoesNotExist;
            }

            var claims = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue.ToString())).ToList();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Email, user.Email)); 
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            var token = PasswordManager.GenerateToken(user.UserName, claims, _jwtSettings);

            return new UserInfo(user.UserName, user.Claims.GetRole(), token);
        }
    }
}