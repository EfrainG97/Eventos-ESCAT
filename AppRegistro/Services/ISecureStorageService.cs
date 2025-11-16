namespace AppRegistro.Services
{
    public interface ISecureStorageService
    {
        Task SaveTokenAsync(string token);
        Task<string?> GetTokenAsync();
        Task SaveApiConfigAsync(string ip, string port);
        Task<(string? ip, string? port)> GetApiConfigAsync();
        Task ClearAllAsync();
    }
}

