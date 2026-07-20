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
                return BadRequest(new { message = "No se pueden hacer predicciones una vez que el partido ha comenzado." });
            }

            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == dto.UserId);
            if (wallet == null)
            {
                return NotFound(new { message = "Billetera no encontrada." });
            }

            if (wallet.Balance < dto.Amount || dto.Amount <= 0)
            {
                return BadRequest(new { message = "Saldo insuficiente o monto inválido." });
            }

            var existingPrediction = await _context.Predictions
                .AnyAsync(p => p.WalletId == wallet.Id && p.MatchId == dto.MatchId);

            if (existingPrediction)
            {
                return BadRequest(new { message = "Ya existe una predicción para este partido." });
            }

            wallet.Balance -= dto.Amount;

            var prediction = new Prediction
            {
                WalletId = wallet.Id,
                MatchId = dto.MatchId,
                PredictedResult = dto.PredictedResult,
                Amount = dto.Amount,
                AppliedOdds = 2.0m, // Fictional fixed odds
                Status = "Pendiente",
                RegistrationDate = DateTime.UtcNow
            };

            _context.Predictions.Add(prediction);
            await _context.SaveChangesAsync();

            var transaction = new Transaction
            {
                WalletId = wallet.Id,
                Amount = -dto.Amount,
                Type = "Predicción",
                PredictionId = prediction.Id,
                Date = DateTime.UtcNow
            };
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Predicción registrada exitosamente.", balance = wallet.Balance, predictionId = prediction.Id });
        }

        // POST: api/Predictions/settle
        [HttpPost("settle")]
        public async Task<ActionResult> SettlePredictions([FromBody] SettlePredictionDto dto)
        {
            var predictions = await _context.Predictions
                .Include(p => p.Wallet)
                .Where(p => p.MatchId == dto.MatchId && p.Status == "Pendiente")
                .ToListAsync();

            if (!predictions.Any())
            {
                return Ok(new { message = "No hay predicciones pendientes por liquidar." });
            }

            foreach (var prediction in predictions)
            {
                if (prediction.PredictedResult == dto.OfficialResult)
                {
                    prediction.Status = "Ganada";
                    decimal prize = prediction.Amount * prediction.AppliedOdds;
                    prediction.Wallet.Balance += prize;

                    var transaction = new Transaction
                    {
                        WalletId = prediction.WalletId,
                        Amount = prize,
                        Type = "Premio",
                        PredictionId = prediction.Id,
                        Date = DateTime.UtcNow
                    };
                    _context.Transactions.Add(transaction);
                }
                else
                {
                    prediction.Status = "Perdida";
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Se han liquidado {predictions.Count} predicciones para el partido {dto.MatchId}." });
        }
        
        // GET: api/Predictions/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PredictionDto>>> GetUserPredictions(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(b => b.UserId == userId);
            if (wallet == null) return NotFound("Billetera no encontrada.");

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
        // GET: api/Predictions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prediction>>> GetPredictions()
        {
            return await _context.Predictions.ToListAsync();
        }

        // GET: api/Predictions/report/most-predicted
        [HttpGet("report/most-predicted")]
        public async Task<ActionResult<IEnumerable<object>>> GetMostPredictedMatches()
        {
            var report = await _context.Predictions
                .GroupBy(p => p.MatchId)
                .Select(g => new
                {
                    matchId = g.Key,
                    totalPredictions = g.Count(),
                    totalAmount = g.Sum(p => p.Amount)
                })
                .OrderByDescending(r => r.totalPredictions)
                .ToListAsync();

            return Ok(report);
        }

        // GET: api/Predictions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Prediction>> GetPrediction(int id)
        {
            var prediction = await _context.Predictions.FindAsync(id);

            if (prediction == null)
            {
                return NotFound();
            }

            return prediction;
        }


    }
}
