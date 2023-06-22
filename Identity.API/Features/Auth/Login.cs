using System.Security.Claims;
using Common;
using Common.Options;
using FluentValidation;
using IdentityServer.Helpers;
using IdentityServer.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Features.Auth;

public class Login
{
    public class Command : IRequest<Result<Response>>
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class Response
    {
        public Response(string userName, string role, string token)
        {
            UserName = userName;
            Role = role;
            Token = token;
        }

        public string UserName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command, Result<Response>>
    {
        private readonly UserDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public Handler(UserDbContext context, JwtSettings jwtSettings)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var users = await _context.Users.Include(u => u.Claims).ToListAsync(cancellationToken);
            var user = users.FirstOrDefault(x => x.UserName == request.UserName) ??
                       users.FirstOrDefault(x => x.Email == request.UserName);
            if (user == null ||
                !PasswordManager.IsValidPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Result.Failure<Response>(DomainErrors.Login.UserDoesNotExist);
            }

            var claims = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            var token = PasswordManager.GenerateToken(user.UserName, claims, _jwtSettings);

            return Result.Success(
                new Response(user.UserName, claims.First(c => c.Type == ClaimTypes.Role).Value, token));
        }
    }
}