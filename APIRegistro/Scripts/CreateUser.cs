// Script de utilidad para crear usuarios en la base de datos
// Ejecutar desde Package Manager Console o terminal:
// dotnet ef migrations add CreateInitialUser
// O usar este código en un endpoint temporal de administración

using APIRegistro.Data;
using APIRegistro.Services;
using Microsoft.EntityFrameworkCore;

namespace APIRegistro.Scripts
{
    public class CreateUserScript
    {
        public static async Task CreateInitialUser(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new ApplicationDbContext(optionsBuilder.Options);

            // Verificar si el usuario ya existe
            var existingUser = await context.Logins.FirstOrDefaultAsync(u => u.User == "admin");
            if (existingUser != null)
            {
                Console.WriteLine("El usuario 'admin' ya existe.");
                return;
            }

            // Crear usuario administrador inicial
            var adminUser = new APIRegistro.Model.Login
            {
                User = "admin",
                PasswordHash = AuthService.HashPassword("Admin123!"), // Cambiar esta contraseña
                Email = "admin@escat.edu.mx",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            context.Logins.Add(adminUser);
            await context.SaveChangesAsync();

            Console.WriteLine("Usuario 'admin' creado exitosamente.");
            Console.WriteLine("IMPORTANTE: Cambia la contraseña por defecto después del primer login.");
        }
    }
}

