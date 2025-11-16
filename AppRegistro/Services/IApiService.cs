using AppRegistro.Models;

namespace AppRegistro.Services
{
    public interface IApiService
    {
        Task<LoginResponse?> LoginAsync(string ip, string port, string user, string password);
        Task<Alumno?> ObtenerAlumnoPorIdentificadorAsync(int identificador);
        Task<bool> ActualizarAlumnoAsync(int identificador, Alumno alumno);
    }
}

