using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IProfileService
{
    Task<BaseResponse> UpdateProfileAsync(string userEmail, UpdateProfileRequest request);
    Task<BaseResponse<UpdateProfileRequest>> GetProfileAsync(string userEmail);
}
