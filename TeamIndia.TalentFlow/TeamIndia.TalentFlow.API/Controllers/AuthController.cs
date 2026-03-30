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
}
