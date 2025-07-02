using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;

namespace LogiTrack.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    public class LogiTrackContext : IdentityDbContext<ApplicationUser>
    {
        public LogiTrackContext(DbContextOptions<LogiTrackContext> options) : base(options) { }

        public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        // Efficiently print order summaries
        public void PrintOrderSummaries()
        {
            var summaries = Orders
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerName,
                    o.DatePlaced,
                    ItemCount = o.Items.Count
                })
                .ToList();

            foreach (var summary in summaries)
            {
                Console.WriteLine($"Order #{summary.OrderId} for {summary.CustomerName} | Items: {summary.ItemCount} | Placed: {summary.DatePlaced:MM/dd/yyyy}");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=logitrack.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensure Identity tables are configured
            // One-to-many: Order has many InventoryItems
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}