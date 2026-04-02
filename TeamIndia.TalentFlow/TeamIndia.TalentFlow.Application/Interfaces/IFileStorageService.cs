using System.IO;

namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Saves a profile image for a user from a stream and returns the public URL (relative path) to the saved file.
    /// </summary>
    Task<string> SaveProfileImageAsync(Guid userId, Stream fileStream, string fileName, string contentType);

    /// <summary>
    /// Deletes a previously saved file given its relative URL.
    /// </summary>
    Task DeleteAsync(string relativeUrl);
}
