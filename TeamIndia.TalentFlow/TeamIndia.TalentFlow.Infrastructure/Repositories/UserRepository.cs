using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;

namespace TeamIndia.TalentFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public Task<ApplicationUser?> FindByEmailAsync(string email)
        => _userManager.FindByEmailAsync(email);

    public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        => _userManager.CreateAsync(user, password);

    public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        => _userManager.CheckPasswordAsync(user, password);

    public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        => _userManager.GetRolesAsync(user);

    public Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        => _userManager.AddToRoleAsync(user, role);

    public Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role)
        => _userManager.RemoveFromRoleAsync(user, role);

    public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role)
        => _userManager.GetUsersInRoleAsync(role);

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<UserProfile?> GetProfileAsync(ApplicationUser user)
    {
        return await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
    }

    public async Task<IdentityResult> CreateOrUpdateProfileAsync(UserProfile profile)
    {
        var existing = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == profile.UserId);
        if (existing == null)
        {
            profile.Id = Guid.NewGuid();
            await _dbContext.UserProfiles.AddAsync(profile);
        }
        else
        {
            existing.Address = profile.Address;
            existing.PostalCode = profile.PostalCode;
            existing.Location = profile.Location;
            existing.DateOfBirth = profile.DateOfBirth;
            existing.Gender = profile.Gender;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        => _userManager.GeneratePasswordResetTokenAsync(user);

    public Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        => _userManager.ResetPasswordAsync(user, token, newPassword);
}
