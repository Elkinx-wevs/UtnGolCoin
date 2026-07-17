using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UTNGolCoin.API.Data;
using UTNGolCoin.API.Models;

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
        public async Task<ActionResult> AssignWelcomeBonus([FromBody] int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == userId);

            if (wallet != null)
            {
                return BadRequest(new { message = "User already has a wallet and received the welcome bonus." });
            }

            // Create new wallet with 10 UTNGolCoin
            wallet = new Wallet
            {
                UserId = userId,
                Balance = 10
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Register transaction
            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Amount = 10,
                Type = "Welcome Bonus",
                Date = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Welcome bonus credited.", balance = wallet.Balance });
        }

        // POST: api/Bonuses/daily
        [HttpPost("daily")]
        public async Task<ActionResult> AssignDailyBonus([FromBody] int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == userId);

            if (wallet == null)
            {
                return NotFound(new { message = "Wallet not found." });
            }

            if (wallet.Balance > 0)
            {
                return BadRequest(new { message = "Balance must be zero to receive the daily anti-bankruptcy bonus." });
            }

            var today = DateTime.UtcNow.Date;
            var alreadyReceivedToday = await _context.DailyBonuses
                .AnyAsync(b => b.WalletId == wallet.Id && b.Date.Date == today);

            if (alreadyReceivedToday)
            {
                return BadRequest(new { message = "You already received the daily bonus today." });
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
                Type = "Daily Anti-Bankruptcy Bonus",
                Date = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Daily bonus of 1 UTNGolCoin credited.", balance = wallet.Balance });
        }
    }
}
