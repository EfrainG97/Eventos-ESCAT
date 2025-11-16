namespace AppRegistro.Models
{
    public class Alumno
    {
        public int IDAlumno { get; set; }
        public int Identificador { get; set; }
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public string? Modalidad { get; set; }
        public int AsisJ { get; set; }
        public int AsisM { get; set; }
        public int AsisConf { get; set; }
    }
}

