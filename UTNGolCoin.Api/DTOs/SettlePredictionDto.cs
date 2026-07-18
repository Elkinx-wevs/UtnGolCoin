namespace UTNGolCoin.API.DTOs
{
    public class SettlePredictionDto
    {
        public int MatchId { get; set; }
        public string OfficialResult { get; set; }
        public decimal AppliedOdds { get; set; }
    }
}
