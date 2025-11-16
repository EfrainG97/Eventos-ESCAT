namespace APIRegistro.DTOs
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Message { get; set; }
        public UserInfo? User { get; set; }
    }

    public class UserInfo
    {
        public int IDLogin { get; set; }
        public string? User { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}

