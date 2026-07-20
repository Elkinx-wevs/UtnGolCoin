using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UTNGolCoin.API.Data;
using UTNGolCoin.API.Models;
using UTNGolCoin.API.DTOs;

namespace UTNGolCoin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BonusesController : ControllerBase
    {
        private readonly UTNGolCoinContext _context;

        public BonusesController(UTNGolCoinContext context)
        {
            _context = context;
        }

        // POST: api/Bonuses/welcome
        [HttpPost("welcome")]
        public async Task<ActionResult> AssignWelcomeBonus([FromBody] UserIdRequestDto request)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == request.UserId);

            if (wallet != null)
            {
                return BadRequest(new { message = "El usuario ya tiene una billetera y ha recibido el bono de bienvenida." });
            }

            // Create new wallet with 10 UTNGolCoin
            wallet = new Wallet
            {
                UserId = request.UserId,
                Balance = 10
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Register transaction
            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Amount = 10,
                Type = "Bono de Bienvenida",
                Date = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bono de bienvenida acreditado.", balance = wallet.Balance });
        }

        // POST: api/Bonuses/daily
        [HttpPost("daily")]
        public async Task<ActionResult> AssignDailyBonus([FromBody] UserIdRequestDto request)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == request.UserId);

            if (wallet == null)
            {
                return NotFound(new { message = "Billetera no encontrada." });
            }

            if (wallet.Balance > 0)
            {
                return BadRequest(new { message = "El saldo debe ser cero para recibir el bono diario anti-bancarrota." });
            }

            var today = DateTime.UtcNow.Date;
            var alreadyReceivedToday = await _context.DailyBonuses
                .AnyAsync(b => b.WalletId == wallet.Id && b.Date.Date == today);

            if (alreadyReceivedToday)
            {
                return BadRequest(new { message = "Ya has recibido el bono diario hoy." });
            }

            // Credit 1 coin
            wallet.Balance += 1;
            
            var dailyBonus = new DailyBonus
            {
                WalletId = wallet.Id,
                Date = DateTime.UtcNow
            };
            _context.DailyBonuses.Add(dailyBonus);

            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Amount = 1,
                Type = "Bono Diario Anti-Bancarrota",
                Date = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Bono diario de 1 UTNGolCoin acreditado.", balance = wallet.Balance });
        }
    }
}
