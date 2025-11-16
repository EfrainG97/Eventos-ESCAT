using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using APIRegistro.Data;
using APIRegistro.DTOs;
using APIRegistro.Model;

namespace APIRegistro.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
        {
            try
            {
                // Buscar usuario en la base de datos
                var user = await _context.Logins
                    .FirstOrDefaultAsync(u => u.User == request.User && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning($"Intento de login fallido: Usuario '{request.User}' no encontrado");
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario o contraseña incorrectos"
                    };
                }

                // Verificar contraseña
                if (!VerifyPassword(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning($"Intento de login fallido: Contraseña incorrecta para usuario '{request.User}'");
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Usuario o contraseña incorrectos"
                    };
                }

                // Actualizar último login
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();

                // Generar token JWT
                var token = await GenerateTokenAsync(user);
                var expiresAt = DateTime.Now.AddHours(Convert.ToDouble(_configuration["JWT:ExpirationHours"] ?? "24"));

                _logger.LogInformation($"Login exitoso para usuario '{request.User}'");

                return new LoginResponse
                {
                    Success = true,
                    Token = token,
                    ExpiresAt = expiresAt,
                    Message = "Autenticación exitosa",
                    User = new UserInfo
                    {
                        IDLogin = user.IDLogin,
                        User = user.User,
                        Email = user.Email,
                        Role = user.Role
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la autenticación");
                return new LoginResponse
                {
                    Success = false,
                    Message = "Error interno del servidor"
                };
            }
        }

        public async Task<string> GenerateTokenAsync(Login user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada")));
            
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IDLogin.ToString()),
                new Claim(ClaimTypes.Name, user.User ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["JWT:ExpirationHours"] ?? "24")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string password, string? passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
                return false;

            // Usar BCrypt para verificar la contraseña
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        // Método auxiliar para hashear contraseñas (útil para crear usuarios)
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }
    }
}

