using System;

namespace UTNGolCoin.API.Models
{
    public class DailyBonus
    {
        public int Id { get; set; }
        
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        
        public DateTime Date { get; set; } // Date when the bonus was given
    }
}
