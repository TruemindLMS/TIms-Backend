using Microsoft.AspNetCore.Mvc;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var res = await _authService.RegisterAsync(request);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var res = await _authService.LoginAsync(request);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var res = await _authService.VerifyOtpAsync(request.Email, request.Code);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string email)
    {
        var res = await _authService.LogoutAsync(email);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var res = await _authService.ForgotPasswordAsync(request);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var email = request.Email;
        var token = request.Token;

        if (string.IsNullOrWhiteSpace(email))
            email = Request.Query["email"].ToString();

        if (string.IsNullOrWhiteSpace(token))
            token = Request.Query["token"].ToString();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            return BadRequest(new { message = "Missing email or token" });

        request.Email = email;
        request.Token = token;

        var res = await _authService.ResetPasswordAsync(request);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] string email)
    {
        var res = await _authService.ResendOtpAsync(email);
        return StatusCode(res.StatusCode, res);
    }
}
