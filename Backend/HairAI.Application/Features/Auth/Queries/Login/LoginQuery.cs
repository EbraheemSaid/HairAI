using MediatR;

namespace HairAI.Application.Features.Auth.Queries.Login;

public class LoginQuery : IRequest<LoginQueryResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}