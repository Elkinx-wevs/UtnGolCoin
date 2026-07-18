using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UTNGolCoin.API.Data;
using UTNGolCoin.API.Models;
using UTNGolCoin.API.DTOs;

namespace UTNGolCoin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        private readonly UTNGolCoinContext _context;

        public PredictionsController(UTNGolCoinContext context)
        {
            _context = context;
        }

        // POST: api/Predictions
        [HttpPost]
        public async Task<ActionResult> CreatePrediction([FromBody] CreatePredictionDto dto)
        {
            if (DateTime.UtcNow >= dto.MatchStartDate)
            {
                return BadRequest(new { message = "Cannot make predictions once the match has started." });
            }

            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == dto.UserId);
            if (wallet == null)
            {
                return NotFound(new { message = "Wallet not found." });
            }

            if (wallet.Balance < dto.Amount || dto.Amount <= 0)
            {
                return BadRequest(new { message = "Insufficient balance or invalid amount." });
            }

            var existingPrediction = await _context.Predictions
                .AnyAsync(p => p.WalletId == wallet.Id && p.MatchId == dto.MatchId);

            if (existingPrediction)
            {
                return BadRequest(new { message = "A prediction already exists for this match." });
            }

            wallet.Balance -= dto.Amount;

            var prediction = new Prediction
            {
                WalletId = wallet.Id,
                MatchId = dto.MatchId,
                PredictedResult = dto.PredictedResult,
                Amount = dto.Amount,
                AppliedOdds = 2.0m, // Fictional fixed odds
                Status = "Pending",
                RegistrationDate = DateTime.UtcNow
            };

            _context.Predictions.Add(prediction);
            await _context.SaveChangesAsync();

            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Amount = -dto.Amount,
                Type = "Prediction",
                PredictionId = prediction.Id,
                Date = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Prediction registered successfully.", balance = wallet.Balance, predictionId = prediction.Id });
        }

        // POST: api/Predictions/settle
        [HttpPost("settle")]
        public async Task<ActionResult> SettlePredictions([FromBody] SettlePredictionDto dto)
        {
            var predictions = await _context.Predictions
                .Include(p => p.Wallet)
                .Where(p => p.MatchId == dto.MatchId && p.Status == "Pending")
                .ToListAsync();

            if (!predictions.Any())
            {
                return Ok(new { message = "No pending predictions to settle." });
            }

            foreach (var prediction in predictions)
            {
                if (prediction.PredictedResult == dto.OfficialResult)
                {
                    prediction.Status = "Won";
                    decimal prize = prediction.Amount * prediction.AppliedOdds;
                    prediction.Wallet.Balance += prize;

                    var transaction = new Transaction
                    {
                        WalletId = prediction.WalletId,
                        Amount = prize,
                        Type = "Prize",
                        PredictionId = prediction.Id,
                        Date = DateTime.UtcNow
                    };
                    _context.Transactions.Add(transaction);
                }
                else
                {
                    prediction.Status = "Lost";
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Settled {predictions.Count} predictions for match {dto.MatchId}." });
        }
        
        // GET: api/Predictions/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PredictionDto>>> GetUserPredictions(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == userId);
            if (wallet == null) return NotFound("Wallet not found.");

            var predictions = await _context.Predictions
                .Where(p => p.WalletId == wallet.Id)
                .OrderByDescending(p => p.RegistrationDate)
                .Select(p => new PredictionDto
                {
                    Id = p.Id,
                    MatchId = p.MatchId,
                    PredictedResult = p.PredictedResult,
                    Amount = p.Amount,
                    AppliedOdds = p.AppliedOdds,
                    Status = p.Status,
                    RegistrationDate = p.RegistrationDate
                })
                .ToListAsync();

            return predictions;
        }
    }
}
