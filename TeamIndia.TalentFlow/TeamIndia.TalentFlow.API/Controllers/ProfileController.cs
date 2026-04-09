using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ICloudinaryService _cloudinaryService;

    public ProfileController(IProfileService profileService, ICloudinaryService cloudinaryService)
    {
        _profileService = profileService;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized();

        var res = await _profileService.GetProfileAsync(email);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update([FromForm] UpdateProfileRequest request)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized();

        if (request.PhotoFile != null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            try
            {
                var url = await _cloudinaryService.UploadProfileImageAsync(request.PhotoFile, userId);
                request.PhotoUrl = url;


            }
            catch (Exception ex)
            {
                return StatusCode(500, TeamIndia.TalentFlow.Application.Common.BaseResponse.Fail("Image upload failed", new[] { ex.Message }, 500));
            }
        }

        var res = await _profileService.UpdateProfileAsync(email, request);
        return StatusCode(res.StatusCode, res);
    }
}
