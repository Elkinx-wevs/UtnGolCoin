using System;

namespace UTNGolCoin.API.Models
{
    public class Prediction
    {
        public int Id { get; set; }
        
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        
        public int MatchId { get; set; } // Reference to Match in the Statistics service
        public string PredictedResult { get; set; } // "1" (Home), "X" (Draw), "2" (Away)
        
        public decimal Amount { get; set; }
        public decimal AppliedOdds { get; set; }
        
        public string Status { get; set; } // "Pending", "Won", "Lost"
        public DateTime RegistrationDate { get; set; }
    }
}
