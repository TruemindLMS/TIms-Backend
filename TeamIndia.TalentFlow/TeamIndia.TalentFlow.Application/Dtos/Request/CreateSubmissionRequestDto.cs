using Microsoft.AspNetCore.Http;

namespace TeamIndia.TalentFlow.Application.Dtos.Request
{
    public class CreateSubmissionRequestDto
    {
        public Guid AssignmentId { get; set; }
        public string? TextResponse { get; set; }
        public IFormFile? File { get; set; }
        public string? LinkUrl { get; set; }
    }
}
