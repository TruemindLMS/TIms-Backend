using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos.Response;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IAuthService
{
    Task<BaseResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<BaseResponse<AuthResponse>> VerifyOtpAsync(string email, string code);
    Task<BaseResponse> LogoutAsync(string email);
    Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequest request);
    Task<BaseResponse> ResendOtpAsync(string email);
}
