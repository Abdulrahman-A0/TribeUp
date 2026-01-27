using Domain.Exceptions.FileExceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ServiceAbstraction.Contracts;
using Shared.Enums;

namespace Service.Implementations
{
    public class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
    {
        public async Task<string> SaveAsync(IFormFile file, MediaType type)
        {
            ValidateFile(file, type);

            var rootPath = ResolvePath(type);

            Directory.CreateDirectory(rootPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(rootPath, fileName);

            try
            {
                await using var stream =
                    new FileStream(path, FileMode.Create);

                await file.CopyToAsync(stream);
            }
            catch
            {
                throw new FileStorageFailedException();
            }

            var relativePath = Path.GetRelativePath(environment.WebRootPath, path);

            return relativePath.Replace("\\", "/");
        }

        public Task DeleteAsync(string relativePath)
        {
            var fullPath = Path.Combine(environment.WebRootPath, relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }


        private static void ValidateFile(IFormFile file, MediaType type)
        {
            var errors = new Dictionary<string, string[]>();

            if (file.Length == 0)
                errors["File"] = new[] { "File is empty" };

            var isVideo = file.ContentType.StartsWith("video/");
            var isImage = file.ContentType.StartsWith("image/");

            switch (type)
            {
                case MediaType.UserProfile:
                case MediaType.GroupProfile:
                    ValidateImage(file, errors);
                    break;

                case MediaType.PostMedia:
                    if (isImage)
                        ValidatePostImage(file, errors);
                    else if (isVideo)
                        ValidatePostVideo(file, errors);
                    else
                        errors["File"] = new[] { "Unsupported media type" };
                    break;
            }

            if (errors.Any())
                throw new FileValidationException(errors);
        }

        private static void ValidateImage(IFormFile file, Dictionary<string, string[]> errors)
        {
            var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image.jpg" };

            if (!allowed.Contains(file.ContentType))
                errors["File"] = new[] { "Unsupported image format" };

            if (file.Length > 2 * 1024 * 1024)
                errors["File"] = new[] { "Image must be under 2MB" };
        }

        private static void ValidatePostImage(IFormFile file, Dictionary<string, string[]> errors)
        {
            if (file.Length > 5 * 1024 * 1024)
                errors["File"] = new[] { "Post image must be under 5MB" };
        }

        private static void ValidatePostVideo(IFormFile file, Dictionary<string, string[]> errors)
        {
            var allowed = new[] { "video/mp4", "video/webm" };

            if (!allowed.Contains(file.ContentType))
                errors["File"] = new[] { "Unsupported video format" };

            if (file.Length > 50 * 1024 * 1024)
                errors["File"] = new[] { "Video must be under 50MB" };
        }

        private string ResolvePath(MediaType type)
        {
            return type switch
            {
                MediaType.PostMedia =>
                    Path.Combine(environment.WebRootPath, "images", "PostUploads"),

                MediaType.UserProfile =>
                    Path.Combine(environment.WebRootPath, "images", "ProfilePictures", "Users"),

                MediaType.GroupProfile =>
                    Path.Combine(environment.WebRootPath, "images", "ProfilePictures", "Groups"),

                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

    }
}
