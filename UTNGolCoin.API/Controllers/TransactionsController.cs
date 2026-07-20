using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UTNGolCoin.API.Data;
using UTNGolCoin.API.Models;
using UTNGolCoin.API.DTOs;

namespace UTNGolCoin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly UTNGolCoinContext _context;

        public TransactionsController(UTNGolCoinContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }


    }
}
