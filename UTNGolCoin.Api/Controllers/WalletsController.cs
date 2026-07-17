using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UTNGolCoin.API.Data;
using UTNGolCoin.API.Models;

namespace UTNGolCoin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly UTNGolCoinContext _context;

        public WalletsController(UTNGolCoinContext context)
        {
            _context = context;
        }

        // GET: api/Wallets/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<Wallet>> GetWallet(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == userId);

            if (wallet == null)
            {
                return NotFound(new { message = "Wallet not found for this user." });
            }

            return wallet;
        }

        // GET: api/Wallets/{userId}/transactions
        [HttpGet("{userId}/transactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == userId);

            if (wallet == null)
            {
                return NotFound(new { message = "Wallet not found." });
            }

            var transactions = await _context.Transactions
                .Where(t => t.WalletId == wallet.Id)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return transactions;
        }
    }
}
