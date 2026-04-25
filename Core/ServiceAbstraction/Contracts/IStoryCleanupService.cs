namespace ServiceAbstraction.Contracts
{
    public interface IStoryCleanupService
    {
        Task CleanupExpiredStoriesAsync();
    }
}
