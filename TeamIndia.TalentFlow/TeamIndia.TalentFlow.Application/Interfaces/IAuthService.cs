using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Common;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IAuthService
{
    Task<BaseResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<BaseResponse<AuthResponse>> VerifyOtpAsync(string email, string code);
}
