using Microsoft.AspNetCore.Identity;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
    Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role);
    Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role);
    Task<IdentityResult> UpdateAsync(ApplicationUser user);
    Task<UserProfile?> GetProfileAsync(ApplicationUser user);
    Task<IdentityResult> CreateOrUpdateProfileAsync(UserProfile profile);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
}
