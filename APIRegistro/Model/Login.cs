namespace APIRegistro.Model
{
    public class Login
    {
        public int IDLogin { get; set; }
        public string? User { get; set; }
        public string? PasswordHash { get; set; } // Hash de la contraseña, nunca texto plano
        public string? Email { get; set; }
        public string? Role { get; set; } // Roles: Admin, User, etc.
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
    }
}
