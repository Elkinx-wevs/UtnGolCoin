using System;

namespace UTNGolCoin.API.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public int? PredictionId { get; set; }
        public DateTime Date { get; set; }
    }
}
