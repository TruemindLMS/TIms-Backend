using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Domain.Enums;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IOnboardingService
{
    Task<BaseResponse> SaveAsync(string userEmail, string? bio, string? profilePictureUrl, Discipline? discipline, Goal? goal);
    Task<BaseResponse> GetOnboardingStatusAsync(string userEmail);
    Task<BaseResponse<string>> GetProfilePictureUrlAsync(string userEmail);
    Task<BaseResponse<string>> UploadProfilePictureForUserAsync(Guid userId, Microsoft.AspNetCore.Http.IFormFile file);
    Task<UserOnboarding?> GetOnboardingAsync(Guid userId);
}
