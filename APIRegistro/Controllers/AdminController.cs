using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIRegistro.Data;
using APIRegistro.Model;
using APIRegistro.Services;

namespace APIRegistro.Controllers
{
    /// Controlador temporal para operaciones de administración
    /// IMPORTANTE: Deshabilitar o proteger este controlador en producción
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// Crea un usuario inicial (solo para desarrollo/setup inicial)
        /// POST /api/admin/create-user
        [HttpPost("create-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var existingUser = await _context.Logins
                    .FirstOrDefaultAsync(u => u.User == request.User || u.Email == request.Email);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "El usuario o email ya existe" });
                }

                var newUser = new Login
                {
                    User = request.User,
                    PasswordHash = AuthService.HashPassword(request.Password),
                    Email = request.Email,
                    Role = request.Role ?? "User",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Logins.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Usuario '{request.User}' creado exitosamente");

                return Ok(new
                {
                    message = "Usuario creado exitosamente",
                    userId = newUser.IDLogin,
                    user = newUser.User
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// Cambia la contraseña de un usuario (solo para desarrollo/setup inicial)
        /// POST /api/admin/change-password
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var user = await _context.Logins
                    .FirstOrDefaultAsync(u => u.User == request.User);

                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                user.PasswordHash = AuthService.HashPassword(request.NewPassword);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Contraseña cambiada para usuario '{request.User}'");

                return Ok(new { message = "Contraseña cambiada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    public class CreateUserRequest
    {
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string User { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}

