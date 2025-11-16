using Microsoft.EntityFrameworkCore;
using APIRegistro.Model;

namespace APIRegistro.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Login> Logins { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la tabla Login
            modelBuilder.Entity<Login>(entity =>
            {
                entity.ToTable("Login");
                entity.HasKey(e => e.IDLogin);
                entity.Property(e => e.User).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("User");
                entity.HasIndex(e => e.User).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuración de la tabla Alumno
            modelBuilder.Entity<Alumno>(entity =>
            {
                entity.ToTable("Alumno");
                entity.HasKey(e => e.IDAlumno);
            });
        }
    }
}

