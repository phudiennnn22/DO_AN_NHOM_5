namespace DOAN.Respository
{
    public interface IChatService
    {
        Task<string> GetBotResponseAsync(string message, CancellationToken cancellationToken = default);
    }
}