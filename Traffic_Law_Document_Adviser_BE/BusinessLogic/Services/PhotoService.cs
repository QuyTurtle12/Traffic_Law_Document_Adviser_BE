using BusinessLogic.Helpers;
using BusinessLogic.IServices;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<(Stream Stream, string ContentType)> GetImageAsync(string url)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return (null, null);

            var contentType = response.Content.Headers.ContentType?.MediaType;
            var stream = await response.Content.ReadAsStreamAsync();

            return (stream, contentType ?? "image/jpeg");
        }
        public async Task<string> UploadImageAsync(IFormFile file, string fileName)
        {
            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".pdf" || extension == ".docx" || extension == ".txt")
                {
                    var rawParams = new RawUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "uploads",
                        PublicId = fileName.Replace(" ", "").ToLower(),
                        UseFilename = false,
                        UniqueFilename = false,
                    };

                    var result = await _cloudinary.UploadAsync(rawParams);
                    return result.SecureUrl.ToString();
                }
                else
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "uploads",
                        PublicId = fileName.Replace(" ", "").ToLower(),
                        UseFilename = false,
                        UniqueFilename = false
                    };

                    var result = await _cloudinary.UploadAsync(uploadParams);
                    return result.SecureUrl.ToString();
                }
            }

            return null;
        }
        public async Task<bool> DeleteImageAsync(string url)
        {
            // Remove version prefix and domain
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // Find publicId from the path (.../uploads/cat123.jpg)
            var folder = segments[^2];
            var file = Path.GetFileNameWithoutExtension(segments[^1]); 

            var publicId = $"{folder}/{file}";
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }
    }
}
