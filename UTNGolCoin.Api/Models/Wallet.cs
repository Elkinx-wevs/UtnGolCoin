using System;

namespace UTNGolCoin.API.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Reference to User in the other service
        public decimal Balance { get; set; }
    }
}
