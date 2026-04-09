using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamIndia.TalentFlow.Application.Dtos.Request;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OnboardingController : ControllerBase
{
    private readonly IOnboardingService _service;
    private readonly ICloudinaryService _cloudinaryService;

    public OnboardingController(IOnboardingService service, ICloudinaryService cloudinaryService)
    {
        _service = service;
        _cloudinaryService = cloudinaryService;
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
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Save([FromForm] CreateOnboardingRequestDto request)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        string? pictureUrl = request.ProfilePictureUrl;
        if (request.ProfilePictureFile != null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            try
            {
                pictureUrl = await _cloudinaryService.UploadProfileImageAsync(request.ProfilePictureFile, userId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, TeamIndia.TalentFlow.Application.Common.BaseResponse.Fail("Image upload failed", new[] { ex.Message }, 500));
            }
        }

        var res = await _service.SaveAsync(email, request.Bio, pictureUrl, request.Discipline, request.Goal);

        if (res != null && res.Success)
        {
            return StatusCode(200, TeamIndia.TalentFlow.Application.Common.BaseResponse<string>.Ok(pictureUrl ?? string.Empty, "Onboarding saved", 200));
        }

        return StatusCode(res.StatusCode, res);
    }
}
