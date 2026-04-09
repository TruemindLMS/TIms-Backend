using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;

        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        [HttpGet("download/{courseId}")]
        public async Task<IActionResult> DownloadCertificate(Guid courseId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            // 1. Check if user actually finished the course
            var isComplete = await _certificateService.IsCourseCompletedAsync(userId, courseId);
            if (!isComplete)
            {
                return BadRequest("Course is not yet completed.");
            }

            // 2. Generate the PDF
            var pdfBytes = await _certificateService.GenerateCertificatePdfAsync(userId, courseId);

            // 3. Return as a file download
            return File(pdfBytes, "application/pdf", $"Certificate_{courseId}.pdf");
        }
    }
}