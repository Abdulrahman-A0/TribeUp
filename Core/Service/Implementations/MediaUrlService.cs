using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ServiceAbstraction.Contracts;
using Shared.Enums;

namespace Service.Implementations
{
    public class MediaUrlService(IConfiguration configuration) : IMediaUrlService
    {
        public string? BuildUrl(string? relativePath, MediaType type)
        {
            var baseUrl = configuration["URLs:BaseUrl"];

            if (string.IsNullOrWhiteSpace(relativePath))
                return type switch
                {
                    MediaType.UserProfile => $"{baseUrl}/images/ProfilePictures/Users/default-user.jpg",
                    MediaType.GroupProfile => $"{baseUrl}/images/ProfilePictures/Groups/default-group.jpg",
                    MediaType.PostMedia => null,
                    MediaType.UserCover => null,
                    _ => null
                };

            return $"{baseUrl}/{relativePath}";
        }
    }
}
