using System.ComponentModel.DataAnnotations;

namespace APIRegistro.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres")]
        public string User { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;
    }
}

