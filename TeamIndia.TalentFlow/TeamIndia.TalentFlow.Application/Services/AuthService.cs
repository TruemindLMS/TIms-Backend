using TeamIndia.TalentFlow.Application.ApplicationSettings;
using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;

namespace TeamIndia.TalentFlow.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, ITokenService tokenService, IOtpService otpService, IEmailService emailService, JwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _otpService = otpService;
        _emailService = emailService;
        _jwtSettings = jwtSettings;
    }

    public async Task<BaseResponse> ResendOtpAsync(string email)
    {
        var user = await _userRepository.FindByEmailAsync(email);
        if (user == null) return BaseResponse.Fail("User not found", null, 404);

        var has = await _otpService.HasValidOtpAsync(email);
        if (has)
        {
            return BaseResponse.Fail("An OTP was recently sent. Please wait until it expires before requesting another.", null, 429);
        }

        var otp = await _otpService.GenerateAndStoreOtpAsync(email);

        try
        {
            var placeholders = new Dictionary<string, string>
            {
                ["FullName"] = user.FullName,
                ["Otp"] = otp,
                ["SupportEmail"] = "support@talentflow.com",
            };

            await _emailService.SendTemplateEmailAsync(user.Email, "Your verification code", "otp-confirmation", placeholders);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send OTP email to {user.Email}: {ex.Message}");
        }

        return BaseResponse.Ok("OTP resent", 200);
    }

    public async Task<BaseResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null) return BaseResponse.Fail("If an account with that email exists, a reset link has been sent.", null, 200);

        var token = await _userRepository.GeneratePasswordResetTokenAsync(user);

        var resetUrl = $"{_jwtSettings.Issuer}/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

        var subject = "Reset your password";

        var placeholders = new Dictionary<string, string>
        {
            ["ResetLink"] = resetUrl,
            ["SupportEmail"] = "support@talentflow.com"
        };

        try
        {
            await _emailService.SendTemplateEmailAsync(user.Email, subject, "reset-password", placeholders);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send reset email: {ex.Message}");
        }

        return BaseResponse.Ok("If an account with that email exists, a reset link has been sent.", 200);
    }

    public async Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmPassword)
            return BaseResponse.Fail("Passwords do not match", null, 400);

        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null) return BaseResponse.Fail("Invalid token or email", null, 400);

        var res = await _userRepository.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!res.Succeeded) return BaseResponse.Fail("Failed to reset password", res.Errors.Select(e => e.Description), 400);

        return BaseResponse.Ok("Password has been reset", 200);
    }

    public async Task<BaseResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword) return BaseResponse<AuthResponse>.Fail("Passwords do not match", null, 400);

        var existing = await _userRepository.FindByEmailAsync(request.Email);
        if (existing != null) return BaseResponse<AuthResponse>.Fail("User already exists", null, 400);

        var role = string.IsNullOrWhiteSpace(request.Role) ? "Intern" : request.Role.Trim();
        var allowed = new[] { "Intern", "Mentor" };
        if (!allowed.Any(r => string.Equals(r, role, System.StringComparison.OrdinalIgnoreCase)))
            return BaseResponse<AuthResponse>.Fail("Invalid role selected", null, 400);

        if (!await _roleRepository.RoleExistsAsync(role))
            return BaseResponse<AuthResponse>.Fail($"Role '{role}' not configured", null, 409);

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName?.Trim() ?? string.Empty,
            EmailConfirmed = false
        };

        var result = await _userRepository.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BaseResponse<AuthResponse>.Fail("Failed to create user", result.Errors.Select(e => e.Description), 400);

        var addRes = await _userRepository.AddToRoleAsync(user, role);
        if (!addRes.Succeeded) return BaseResponse<AuthResponse>.Fail("Failed to assign role", addRes.Errors.Select(e => e.Description), 500);

        if (string.Equals(role, "Mentor", StringComparison.OrdinalIgnoreCase))
        {
            user.IsMentorApproved = false;
            await _userRepository.UpdateAsync(user);
        }

        var otp = await _otpService.GenerateAndStoreOtpAsync(user.Email);

        // send OTP to user's email using template
        try
        {
            var placeholders = new Dictionary<string, string>
            {
                ["FullName"] = user.FullName,
                ["Otp"] = otp,
                ["SupportEmail"] = "support@talentflow.com",
            };

            await _emailService.SendTemplateEmailAsync(user.Email, "Verify your email", "otp-confirmation", placeholders);
        }
        catch (Exception ex)
        {
            // log and continue — user was created, OTP stored; email failure should not expose OTP
            Console.WriteLine($"Failed to send OTP email to {user.Email}: {ex.Message}");
        }

        return BaseResponse<AuthResponse>.Ok(new AuthResponse { Token = string.Empty, ExpiresAtUtc = DateTime.UtcNow, Email = user.Email, FullName = user.FullName }, "Registration successful, OTP sent to email", 202);
    }

    public async Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null) return BaseResponse<AuthResponse>.Fail("Invalid credentials", null, 401);

        var valid = await _userRepository.CheckPasswordAsync(user, request.Password);
        if (!valid) return BaseResponse<AuthResponse>.Fail("Invalid password", null, 401);

        if (!user.EmailConfirmed) return BaseResponse<AuthResponse>.Fail("Email not verified", null, 401);

        var roles = await _userRepository.GetRolesAsync(user);
        var token = await _tokenService.GenerateTokenAsync(user, roles);

        return BaseResponse<AuthResponse>.Ok(new AuthResponse { Token = token, ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes), Email = user.Email, FullName = user.FullName }, "Logged in", 200);
    }

    public async Task<BaseResponse<AuthResponse>> VerifyOtpAsync(string email, string code)
    {
        var user = await _userRepository.FindByEmailAsync(email);
        if (user == null) return BaseResponse<AuthResponse>.Fail("User not found", null, 404);

        var ok = await _otpService.VerifyOtpAsync(email, code);
        if (!ok) return BaseResponse<AuthResponse>.Fail("Invalid or expired OTP", null, 400);

        user.EmailConfirmed = true;
        var up = await _userRepository.UpdateAsync(user);
        if (!up.Succeeded) return BaseResponse<AuthResponse>.Fail("Failed to update user", up.Errors.Select(e => e.Description), 500);

        var roles = await _userRepository.GetRolesAsync(user);
        var token = await _tokenService.GenerateTokenAsync(user, roles);

        return BaseResponse<AuthResponse>.Ok(new AuthResponse { Token = token, ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes), Email = user.Email, FullName = user.FullName }, "Email Verified", 200);
    }

    public async Task<BaseResponse> LogoutAsync(string email)
    {
        return BaseResponse.Ok("Logged out", 200);
    }
}
