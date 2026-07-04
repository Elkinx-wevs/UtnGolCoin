using Microsoft.EntityFrameworkCore;
using UTNGolCoin.API.Data;
using UTNGolCoin.API.Models;
using BCrypt.Net;

namespace UTNGolCoin.API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UTNGolCoinContext _context;

        public class DuplicateEmailException : Exception
        {
            public DuplicateEmailException(string message) : base(message) { }
        }

        public UsuarioService(UTNGolCoinContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .ToListAsync();
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> CreateUsuarioAsync(Usuario usuario)
        {
            bool emailExists = await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo);
            if (emailExists)
            {
                throw new DuplicateEmailException("El correo ya está registrado.");
            }

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);

            if (usuario.FechaRegistro == default)
            {
                usuario.FechaRegistro = DateTime.UtcNow;
            }
            else
            {
                usuario.FechaRegistro = DateTime.SpecifyKind(usuario.FechaRegistro, DateTimeKind.Utc);
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<bool> UpdateUsuarioAsync(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return false;
            }

            var existingUsuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (existingUsuario == null)
            {
                return false;
            }

            if (existingUsuario.Correo != usuario.Correo)
            {
                bool emailExists = await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo && u.Id != id);
                if (emailExists)
                {
                    throw new DuplicateEmailException("El correo ya está registrado por otro usuario.");
                }
            }

            if (!string.IsNullOrEmpty(usuario.PasswordHash) &&
                !usuario.PasswordHash.StartsWith("$2a$") &&
                !usuario.PasswordHash.StartsWith("$2b$") &&
                !usuario.PasswordHash.StartsWith("$2y$"))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
            }

            usuario.FechaRegistro = DateTime.SpecifyKind(usuario.FechaRegistro, DateTimeKind.Utc);

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UsuarioExistsAsync(id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return false;
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> UsuarioExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(e => e.Id == id);
        }
    }
}
