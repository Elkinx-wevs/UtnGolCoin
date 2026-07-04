using Microsoft.EntityFrameworkCore;
using UTNGolCoin.API.Data;
using UTNGolCoin.API.Models;

namespace UTNGolCoin.API.Services
{
    public class PartidoService : IPartidoService
    {
        private readonly UTNGolCoinContext _context;

        public class InvalidMatchTeamsException : Exception
        {
            public InvalidMatchTeamsException(string message) : base(message) { }
        }

        public PartidoService(UTNGolCoinContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Partido>> GetPartidosAsync()
        {
            return await _context.Partidos
                .Include(p => p.SeleccionLocal)
                .Include(p => p.SeleccionVisitante)
                .Include(p => p.Sede)
                .ToListAsync();
        }

        public async Task<Partido?> GetPartidoByIdAsync(int id)
        {
            return await _context.Partidos
                .Include(p => p.SeleccionLocal)
                .Include(p => p.SeleccionVisitante)
                .Include(p => p.Sede)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Partido> CreatePartidoAsync(Partido partido)
        {
            if (partido.SeleccionLocalId == partido.SeleccionVisitanteId)
            {
                throw new InvalidMatchTeamsException("Una selección no puede jugar contra sí misma.");
            }

            if (partido.FechaHora == default)
            {
                partido.FechaHora = DateTime.UtcNow;
            }
            else
            {
                partido.FechaHora = DateTime.SpecifyKind(partido.FechaHora, DateTimeKind.Utc);
            }

            _context.Partidos.Add(partido);
            await _context.SaveChangesAsync();

            return partido;
        }

        public async Task<bool> UpdatePartidoAsync(int id, Partido partido)
        {
            if (id != partido.Id)
            {
                return false;
            }

            if (partido.SeleccionLocalId == partido.SeleccionVisitanteId)
            {
                throw new InvalidMatchTeamsException("Una selección no puede jugar contra sí misma.");
            }

            partido.FechaHora = DateTime.SpecifyKind(partido.FechaHora, DateTimeKind.Utc);

            _context.Entry(partido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PartidoExistsAsync(id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeletePartidoAsync(int id)
        {
            var partido = await _context.Partidos.FindAsync(id);
            if (partido == null)
            {
                return false;
            }

            _context.Partidos.Remove(partido);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> PartidoExistsAsync(int id)
        {
            return await _context.Partidos.AnyAsync(e => e.Id == id);
        }
    }
}
