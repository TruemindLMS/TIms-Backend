using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TeamIndia.TalentFlow.Application.ApplicationSettings;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.Application.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> settings)
    {
        var acc = new Account(
            settings.Value.CloudName,
            settings.Value.ApiKey,
            settings.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadCourseVideoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        var publicId = Guid.NewGuid().ToString();
        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            Folder = "courses",
            PublicId = publicId
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result.SecureUrl.AbsoluteUri;
    }

    public async Task<string> UploadProfileImageAsync(IFormFile file, string userId)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            Folder = "profiles",
            PublicId = userId,
            Overwrite = true
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result.SecureUrl.AbsoluteUri;
    }
}