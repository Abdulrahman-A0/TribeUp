using Shared.Enums;

namespace ServiceAbstraction.Contracts
{
    public interface IMediaUrlService
    {
        string BuildUrl(string? relativePath, MediaType type);
    }
}
