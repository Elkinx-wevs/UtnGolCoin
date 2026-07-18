using System;

namespace UTNGolCoin.API.DTOs
{
    public class PredictionDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public string PredictedResult { get; set; }
        public decimal Amount { get; set; }
        public decimal AppliedOdds { get; set; }
        public string Status { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
