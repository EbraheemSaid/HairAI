using MediatR;
using HairAI.Application.Common.Interfaces;

namespace HairAI.Application.Features.Auth.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginQueryResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;

    public LoginQueryHandler(IIdentityService identityService, IJwtService jwtService)
    {
        _identityService = identityService;
        _jwtService = jwtService;
    }

    public async Task<LoginQueryResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var (success, user, message) = await _identityService.AuthenticateAsync(request.Email, request.Password);

        if (!success || user == null)
        {
            return new LoginQueryResponse
            {
                Success = false,
                Message = message,
                Errors = new List<string> { message }
            };
        }

        // Get user roles
        var roles = await _identityService.GetUserRolesAsync(user);

        // Generate JWT token
        var token = _jwtService.GenerateToken(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.ClinicId,
            roles);

        return new LoginQueryResponse
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ClinicId = user.ClinicId
        };
    }
}