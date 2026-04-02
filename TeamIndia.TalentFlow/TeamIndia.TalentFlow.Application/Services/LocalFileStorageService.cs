using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using TeamIndia.TalentFlow.Application.Interfaces;
namespace TeamIndia.TalentFlow.Application.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _webRootPath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(string webRootPath, ILogger<LocalFileStorageService> logger)
    {
        _webRootPath = string.IsNullOrWhiteSpace(webRootPath) ? "wwwroot" : webRootPath;
        _logger = logger;
    }

    public async Task<string> SaveProfileImageAsync(Guid userId, Stream fileStream, string fileName, string contentType)
    {
        if (fileStream == null) throw new ArgumentException("File stream is empty", nameof(fileStream));

        // validate content type
        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowed.Contains(contentType))
            throw new ArgumentException("Unsupported file type", nameof(contentType));

        var uploads = Path.Combine(_webRootPath, "uploads", "users", userId.ToString());
        Directory.CreateDirectory(uploads);

        var ext = Path.GetExtension(fileName);
        var filename = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(uploads, filename);

        // load image and resize to reasonable dimensions to save storage (max 1200x1200)
        try
        {
            using var image = await Image.LoadAsync(fileStream);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new SixLabors.ImageSharp.Size(1200, 1200)
            }));

            var encoder = new JpegEncoder { Quality = 85 };
            await image.SaveAsync(path, encoder);
        }
        catch
        {
            // fallback: save raw stream
            fileStream.Position = 0;
            using var stream = File.Create(path);
            await fileStream.CopyToAsync(stream);
        }

        var relative = $"/uploads/users/{userId}/{filename}";
        return relative;
    }

    public Task DeleteAsync(string relativeUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(relativeUrl)) return Task.CompletedTask;
            var trimmed = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var full = Path.Combine(_webRootPath, trimmed);
            if (File.Exists(full)) File.Delete(full);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete file {Url}", relativeUrl);
        }

        return Task.CompletedTask;
    }
}
