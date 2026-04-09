using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Application.Dtos.Response;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserFullResponseDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var list = new List<UserFullResponseDto>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            var profile = await _userRepository.GetProfileAsync(u);
            var onboarding = await _userRepository.GetOnboardingAsync(u.Id);

            list.Add(new UserFullResponseDto
            {
                UserId = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Roles = roles,
                Profile = profile == null ? null : new ProfileDto
                {
                    Id = profile.Id,
                    UserId = profile.UserId,
                    Address = profile.Address,
                    PostalCode = profile.PostalCode,
                    Location = profile.Location,
                    DateOfBirth = profile.DateOfBirth,
                    Gender = profile.Gender,
                    UpdatedAtUtc = profile.UpdatedAtUtc
                },
                Onboarding = onboarding == null ? null : new OnboardingDto
                {
                    Id = onboarding.Id,
                    UserId = onboarding.UserId,
                    Bio = onboarding.Bio,
                    ProfilePictureUrl = onboarding.ProfilePictureUrl,
                    Discipline = onboarding.Discipline,
                    Goal = onboarding.Goal,
                    IsComplete = onboarding.IsComplete,
                    UpdatedAtUtc = onboarding.UpdatedAtUtc
                }
            });
        }

        return list;
    }

    public async Task<UserFullResponseDto?> GetUserByIdAsync(Guid userId)
    {
        var u = await _userRepository.GetByIdAsync(userId);
        if (u == null) return null;

        var roles = await _userManager.GetRolesAsync(u);
        var profile = await _userRepository.GetProfileAsync(u);
        var onboarding = await _userRepository.GetOnboardingAsync(u.Id);

        return new UserFullResponseDto
        {
            UserId = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            Roles = roles,
            Profile = profile == null ? null : new ProfileDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                Address = profile.Address,
                PostalCode = profile.PostalCode,
                Location = profile.Location,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                UpdatedAtUtc = profile.UpdatedAtUtc
            },
            Onboarding = onboarding == null ? null : new OnboardingDto
            {
                Id = onboarding.Id,
                UserId = onboarding.UserId,
                Bio = onboarding.Bio,
                ProfilePictureUrl = onboarding.ProfilePictureUrl,
                Discipline = onboarding.Discipline,
                Goal = onboarding.Goal,
                IsComplete = onboarding.IsComplete,
                UpdatedAtUtc = onboarding.UpdatedAtUtc
            }
        };
    }
}
