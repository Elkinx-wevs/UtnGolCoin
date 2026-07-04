using UTNGolCoin.API.Models;

namespace UTNGolCoin.API.Services
{
    public interface IPartidoService
    {
        Task<IEnumerable<Partido>> GetPartidosAsync();
        Task<Partido?> GetPartidoByIdAsync(int id);
        Task<Partido> CreatePartidoAsync(Partido partido);
        Task<bool> UpdatePartidoAsync(int id, Partido partido);
        Task<bool> DeletePartidoAsync(int id);
    }
}
