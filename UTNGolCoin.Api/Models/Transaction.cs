using System;

namespace UTNGolCoin.API.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        
        public decimal Amount { get; set; }
        public string Type { get; set; } // Welcome Bonus, Prediction, Prize, Daily Bonus
        public DateTime Date { get; set; }
        
        // Optional reference to a prediction if the transaction is related to one
        public int? PredictionId { get; set; }
    }
}
