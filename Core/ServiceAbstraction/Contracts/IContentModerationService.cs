using ServiceAbstraction.Models;

namespace ServiceAbstraction.Contracts
{
    public interface IContentModerationService
    {
        Task<ModerationResult> AnalyzeAsync(string text);
    }
}
