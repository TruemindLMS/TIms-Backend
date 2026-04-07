using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OnboardingController : ControllerBase
{
    private readonly IOnboardingService _service;

    public OnboardingController(IOnboardingService service)
    {
        _service = service;
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();
        var res = await _service.GetOnboardingStatusAsync(email);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromForm] string? bio, [FromForm] string? profilePictureUrl, [FromForm] Discipline? discipline, [FromForm] Goal? goal)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var res = await _service.SaveAsync(email, bio, profilePictureUrl, discipline, goal);
        return StatusCode(res.StatusCode, res);
    }
}
