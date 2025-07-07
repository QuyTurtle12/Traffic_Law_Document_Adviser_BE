using Microsoft.AspNetCore.Http;

namespace BusinessLogic.IServices
{
    public interface IPhotoService
    {
        Task<string> UploadImageAsync(IFormFile file, string fileName);
        Task<(Stream Stream, string ContentType)> GetImageAsync(string url);
        Task<bool> DeleteImageAsync(string url);
        Task<(Stream Stream, string ContentType)> DownloadRawAsync(string url);

    }
}
