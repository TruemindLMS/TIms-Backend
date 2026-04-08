using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamIndia.TalentFlow.Application.Common;
using TeamIndia.TalentFlow.Application.Dtos;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]

public class CloudinaryController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;

    public CloudinaryController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("upload-course-video")]
    [Consumes("multipart/form-data")]

    public async Task<IActionResult> UploadCourseVideo([FromForm] CourseVideoUploadRequest videoUploadRequest)
    {
        if (videoUploadRequest.File == null)
            return BadRequest(BaseResponse.Fail("File is required", null, 400));

        try
        {
            var url = await _cloudinaryService.UploadCourseVideoAsync(videoUploadRequest.File);
            return StatusCode(200, BaseResponse<string>.Ok(url, "Uploaded", 200));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse.Fail("Upload failed", new[] { ex.Message }, 500));
        }
    }

    [Authorize]
    [HttpPost("upload-profile-image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfileImage([FromForm] ProfileImageUploadRequest profileImageUploadRequest)
    {
        if (profileImageUploadRequest.File == null)
            return BadRequest(BaseResponse.Fail("File is required", null, 400));

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(BaseResponse.Fail("User not identified", null, 401));

        try
        {
            var url = await _cloudinaryService.UploadProfileImageAsync(profileImageUploadRequest.File, userId);
            return StatusCode(200, BaseResponse<string>.Ok(url, "Uploaded", 200));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse.Fail("Upload failed", new[] { ex.Message }, 500));
        }
    }
}
