using System.Security.Claims;
using Common;
using FluentValidation;
using IdentityServer.Extensions;
using IdentityServer.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Features;

public class GetUser
{
    public class Query : IRequest<Result<Response>>
    {
        public string UserId { get; set; } = null!;
    }

    public class Response
    {
        public Response(string userId, string email, string userName, Role role)
        {
            UserId = userId;
            Email = email;
            UserName = userName;
            Role = role;
        }

        public string UserId { get; }
        public string Email { get; }
        public string UserName { get; }
        public Role Role { get; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Query, Result<Response>>
    {
        private readonly UserDbContext _context;

        public Handler(UserDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Include(u => u.Claims)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (user == null)
            {
                return DomainErrors.GetUser.UserDoesNotExist;
            }

            return new Response(user.Id, user.Email, user.UserName, user.Claims.GetRole());
        }
    }
}