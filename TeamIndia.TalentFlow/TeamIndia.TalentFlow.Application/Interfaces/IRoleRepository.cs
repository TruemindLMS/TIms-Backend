using Microsoft.AspNetCore.Identity;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IRoleRepository
{
    Task<bool> RoleExistsAsync(string roleName);
    Task<IdentityResult> CreateAsync(ApplicationRole role);
}
