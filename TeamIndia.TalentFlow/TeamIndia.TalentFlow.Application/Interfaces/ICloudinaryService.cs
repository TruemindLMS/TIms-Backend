using Microsoft.AspNetCore.Http;

namespace TeamIndia.TalentFlow.Application.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadCourseVideoAsync(IFormFile file);
        Task<string> UploadProfileImageAsync(IFormFile file, string userId);
        Task<string> UploadAssignmentFileAsync(IFormFile file, string userId, Guid assignmentId);
        Task<string> UploadRawAsync(System.IO.Stream stream, string fileName, string folder, string publicId);
    }
}
