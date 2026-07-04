using Microsoft.AspNetCore.Mvc;
using UTNGolCoin.API.Models;
using UTNGolCoin.API.Services;

namespace UTNGolCoin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidosController : ControllerBase
    {
        private readonly IPartidoService _partidoService;

        public PartidosController(IPartidoService partidoService)
        {
            _partidoService = partidoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Partido>>> GetPartidos()
        {
            var partidos = await _partidoService.GetPartidosAsync();
            return Ok(partidos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Partido>> GetPartido(int id)
        {
            var partido = await _partidoService.GetPartidoByIdAsync(id);
            if (partido == null)
            {
                return NotFound();
            }
            return Ok(partido);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPartido(int id, Partido partido)
        {
            try
            {
                var result = await _partidoService.UpdatePartidoAsync(id, partido);
                if (!result)
                {
                    return BadRequest("No se pudo actualizar el partido. Verifique el ID.");
                }
                return NoContent();
            }
            catch (PartidoService.InvalidMatchTeamsException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Partido>> PostPartido(Partido partido)
        {
            try
            {
                var createdPartido = await _partidoService.CreatePartidoAsync(partido);
                return CreatedAtAction("GetPartido", new { id = createdPartido.Id }, createdPartido);
            }
            catch (PartidoService.InvalidMatchTeamsException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartido(int id)
        {
            var result = await _partidoService.DeletePartidoAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
