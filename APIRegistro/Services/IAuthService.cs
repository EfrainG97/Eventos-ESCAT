using APIRegistro.DTOs;
using APIRegistro.Model;

namespace APIRegistro.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> AuthenticateAsync(LoginRequest request);
        Task<string> GenerateTokenAsync(Login user);
    }
}

