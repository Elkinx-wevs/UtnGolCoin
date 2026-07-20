using Microsoft.EntityFrameworkCore;
using UTNGolCoin.API.Models;

namespace UTNGolCoin.API.Data
{
    public class UTNGolCoinContext : DbContext
    {
        public UTNGolCoinContext(DbContextOptions<UTNGolCoinContext> options) : base(options)
        {
        }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<DailyBonus> DailyBonuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            NormalizeDateTimes();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            NormalizeDateTimes();
            return base.SaveChanges();
        }

        private void NormalizeDateTimes()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                foreach (var prop in entry.Properties)
                {
                    var clrType = Nullable.GetUnderlyingType(prop.Metadata.ClrType) ?? prop.Metadata.ClrType;
                    if (clrType == typeof(DateTime) && prop.CurrentValue is DateTime dt && dt.Kind == DateTimeKind.Unspecified)
                    {
                        prop.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                }
            }
        }
    }
}
