using Microsoft.AspNetCore.Http;

namespace TeamIndia.TalentFlow.Application.Dtos
{
    public class CourseVideoUploadRequest
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
