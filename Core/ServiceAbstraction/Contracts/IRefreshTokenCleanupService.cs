namespace ServiceAbstraction.Contracts
{
    public interface IRefreshTokenCleanupService
    {
        Task CleanupExpiredTokensAsync();
    }
}
