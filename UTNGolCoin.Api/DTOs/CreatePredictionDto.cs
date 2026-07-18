using System;

namespace UTNGolCoin.API.DTOs
{
    public class CreatePredictionDto
    {
        public int UserId { get; set; }
        public int MatchId { get; set; }
        public string PredictedResult { get; set; }
        public decimal Amount { get; set; }
        public DateTime MatchStartDate { get; set; }
    }
}
