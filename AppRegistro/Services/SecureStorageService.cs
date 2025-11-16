namespace AppRegistro.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        private const string TokenKey = "bearer_token";
        private const string ApiIpKey = "api_ip";
        private const string ApiPortKey = "api_port";

        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await SecureStorage.GetAsync(TokenKey);
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveApiConfigAsync(string ip, string port)
        {
            await SecureStorage.SetAsync(ApiIpKey, ip);
            await SecureStorage.SetAsync(ApiPortKey, port);
        }

        public async Task<(string? ip, string? port)> GetApiConfigAsync()
        {
            try
            {
                var ip = await SecureStorage.GetAsync(ApiIpKey);
                var port = await SecureStorage.GetAsync(ApiPortKey);
                return (ip, port);
            }
            catch
            {
                return (null, null);
            }
        }

        public async Task ClearAllAsync()
        {
            SecureStorage.Remove(TokenKey);
            SecureStorage.Remove(ApiIpKey);
            SecureStorage.Remove(ApiPortKey);
            await Task.CompletedTask;
        }
    }
}

