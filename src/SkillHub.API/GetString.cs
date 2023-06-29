using Carter;
using FluentValidation;
using MediatR;

namespace SkillHub.API;

public class GetString : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/categories", (IMediator mediator, Command command) => mediator.Send(command))
            .WithName(nameof(GetString))
            .WithTags(nameof(Command));
    }

    public class Command : IRequest<string>
    {
        public string? Title { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Title)
                .MaximumLength(200)
                .NotEmpty();
        }
    }

    internal sealed class CreateTodoItemCommandHandler : IRequestHandler<Command, string>
    {
        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.Title == "test")
            {
                throw new Exception("test");
            }

            return request.Title.ToUpper();
        }
    }
}