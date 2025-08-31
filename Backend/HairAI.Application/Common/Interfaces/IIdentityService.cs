using HairAI.Domain.Entities;
using HairAI.Application.Common;

namespace HairAI.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
    Task<(Result Result, string UserId)> CreateUserAsync(string email, string password, string firstName, string lastName, Guid? clinicId = null);
    Task<Result> DeleteUserAsync(string userId);
    Task<Result> AssignRoleAsync(string userId, string role);
    Task<(bool Success, ApplicationUser? User, string Message)> AuthenticateAsync(string email, string password);
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
}