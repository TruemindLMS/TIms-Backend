using Microsoft.AspNetCore.Identity;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleRepository(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public Task<bool> RoleExistsAsync(string roleName)
        => _roleManager.RoleExistsAsync(roleName);

    public Task<IdentityResult> CreateAsync(ApplicationRole role)
        => _roleManager.CreateAsync(role);
}
