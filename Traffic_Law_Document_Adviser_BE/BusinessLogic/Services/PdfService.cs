using BusinessLogic.Helpers;
using BusinessLogic.IServices;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Services
{
    public class PdfService : IPdfService
    {
        private readonly Cloudinary _cloudinary;

        public PdfService(IOptions<CloudinarySettings> config)
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
            if (!url.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return (null, null);

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return (null, null);

            var contentType = response.Content.Headers.ContentType?.MediaType;
            var stream = await response.Content.ReadAsStreamAsync();

            return (stream, contentType ?? "application/pdf");
        }
        public async Task<string> UploadImageAsync(IFormFile file, string fileName)
        {
            if (file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".pdf")
                {
                    await using var stream = file.OpenReadStream();

                    // Remove extension from fileName if present
                    fileName = Path.GetFileNameWithoutExtension(fileName)
                        .Replace(" ", "")
                        .ToLower();

                    var rawParams = new RawUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "uploads",
                        PublicId = fileName,
                        UseFilename = false,
                        UniqueFilename = false,
                        Overwrite = true
                    };

                    var result = await _cloudinary.UploadAsync(rawParams);
                    return result.SecureUrl.ToString();
                }
            }

            return null;
        }
        public async Task<bool> DeleteImageAsync(string url)
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            var folder = segments[^2];
            var file = Path.GetFileName(segments[^1]);

            var publicId = $"{folder}/{Path.GetFileName(file)}";

            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            };

            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        public async Task<(Stream Stream, string ContentType)> DownloadFileAsync(string url)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (null, null);

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var stream = await response.Content.ReadAsStreamAsync();
            return (stream, contentType);
        }


    }
}
