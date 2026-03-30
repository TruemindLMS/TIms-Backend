using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _role_repository;

    public AdminService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _role_repository = roleRepository;
    }

    public async Task<BaseResponse> ApproveMentorAsync(string email)
    {
        var user = await _userRepository.FindByEmailAsync(email);
        if (user == null) return BaseResponse.Fail("User not found", null, 404);

        var roles = await _userRepository.GetRolesAsync(user);
        if (!roles.Any(r => string.Equals(r, "Mentor", StringComparison.OrdinalIgnoreCase)))
            return BaseResponse.Fail("User is not registered as Mentor", null, 400);

        user.IsMentorApproved = true;
        var res = await _userRepository.UpdateAsync(user);
        if (!res.Succeeded) return BaseResponse.Fail("Failed to approve mentor", res.Errors.Select(e => e.Description), 500);

        return BaseResponse.Ok("Mentor approved", 200);
    }

    public async Task<BaseResponse> RejectMentorAsync(string email)
    {
        var user = await _userRepository.FindByEmailAsync(email);
        if (user == null) return BaseResponse.Fail("User not found", null, 404);

        var roles = await _userRepository.GetRolesAsync(user);
        if (roles.Any(r => string.Equals(r, "Mentor", StringComparison.OrdinalIgnoreCase)))
        {
            await _userRepository.RemoveFromRoleAsync(user, "Mentor");
        }

        user.IsMentorApproved = false;
        var res = await _userRepository.UpdateAsync(user);
        if (!res.Succeeded) return BaseResponse.Fail("Failed to reject mentor", res.Errors.Select(e => e.Description), 500);

        return BaseResponse.Ok("Mentor rejected", 200);
    }
}
