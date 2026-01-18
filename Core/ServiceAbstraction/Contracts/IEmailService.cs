namespace ServiceAbstraction.Contracts
{
    public interface IEmailService
    {
        Task SendPasswordResetAsync(string email, string resetLink);
    }
}
