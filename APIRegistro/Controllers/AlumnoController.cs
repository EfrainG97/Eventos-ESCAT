using APIRegistro.Data;
using APIRegistro.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIRegistro.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlumnoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alumno>>> GetAlumnos()
        {
            var alumnos = await _context.Alumnos.ToListAsync();
            return Ok(alumnos); // Más explícito
        }



        [HttpGet("{identificador}")]
        public async Task<IActionResult> GetAlumnoByIdentificador(int identificador)
        {
            var alumno = await _context.Alumnos
                .FirstOrDefaultAsync(a => a.Identificador == identificador);

            if (alumno == null)
            {
                return NotFound(new { mensaje = "Alumno no encontrado" });
            }

            return Ok(alumno);
        }

        [HttpPut("{identificador}")]
        public async Task<IActionResult> PutAlumno(int identificador, Alumno alumno)
        {
            if (identificador != alumno.Identificador)
            {
                return BadRequest(new { mensaje = "El identificador no coincide" });
            }

            _context.Entry(alumno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Alumnos.Any(e => e.Identificador == identificador))
                {
                    return NotFound(new { mensaje = "Alumno no encontrado" });
                }
                else
                {
                    throw;
                }
            }
            
            return NoContent();
        }
    }
}
