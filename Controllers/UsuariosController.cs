using Microsoft.AspNetCore.Mvc;
using UTNGolCoin.API.Models;
using UTNGolCoin.API.Services;

namespace UTNGolCoin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _usuarioService.GetUsuariosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            try
            {
                var result = await _usuarioService.UpdateUsuarioAsync(id, usuario);
                if (!result)
                {
                    return BadRequest("No se pudo actualizar el usuario. Verifique el ID.");
                }
                return NoContent();
            }
            catch (UsuarioService.DuplicateEmailException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            try
            {
                var createdUsuario = await _usuarioService.CreateUsuarioAsync(usuario);
                return CreatedAtAction("GetUsuario", new { id = createdUsuario.Id }, createdUsuario);
            }
            catch (UsuarioService.DuplicateEmailException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var result = await _usuarioService.DeleteUsuarioAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
