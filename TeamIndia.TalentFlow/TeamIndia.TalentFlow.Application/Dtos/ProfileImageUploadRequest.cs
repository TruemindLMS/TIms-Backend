using Microsoft.AspNetCore.Http;

namespace TeamIndia.TalentFlow.Application.Dtos
{
    public class ProfileImageUploadRequest
    {
        public IFormFile File { get; set; }

    }
}
