using System.Security.Claims;

namespace HairAI.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string email, string firstName, string lastName, Guid? clinicId = null, IList<string>? roles = null);
    ClaimsPrincipal? ValidateToken(string token);
} 