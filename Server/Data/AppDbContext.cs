using Microsoft.EntityFrameworkCore;
using MyApp.Shared.Domain;

namespace MyApp.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockReceipt> StockReceipts => Set<StockReceipt>();
    public DbSet<StockReceiptLine> StockReceiptLines => Set<StockReceiptLine>();
    public DbSet<StockIssue> StockIssues => Set<StockIssue>();
    public DbSet<StockIssueLine> StockIssueLines => Set<StockIssueLine>();
    public DbSet<InventoryLedgerEntry> InventoryLedgerEntries => Set<InventoryLedgerEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(300);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products", table =>
            {
                table.HasCheckConstraint("CK_Products_OnHandQty_NonNegative", "[OnHandQty] >= 0");
                table.HasCheckConstraint("CK_Products_ReorderLevel_NonNegative", "[ReorderLevel] >= 0");
            });
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Sku).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.AverageCost).HasPrecision(18, 2);
            entity.HasIndex(x => x.Sku).IsUnique();
            entity.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockReceipt>(entity =>
        {
            entity.ToTable("StockReceipts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DocumentNo).IsRequired().HasMaxLength(50);
            entity.HasIndex(x => x.DocumentNo).IsUnique();
            entity.Property(x => x.Supplier).HasMaxLength(200);
            entity.Property(x => x.Note).HasMaxLength(500);
            entity.Property(x => x.TotalAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<StockReceiptLine>(entity =>
        {
            entity.ToTable("StockReceiptLines", table =>
            {
                table.HasCheckConstraint("CK_StockReceiptLines_Quantity_Positive", "[Quantity] > 0");
                table.HasCheckConstraint("CK_StockReceiptLines_UnitCost_NonNegative", "[UnitCost] >= 0");
            });
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UnitCost).HasPrecision(18, 2);
            entity.Property(x => x.LineTotal).HasPrecision(18, 2);
            entity.HasOne(x => x.StockReceipt)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.StockReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Product)
                .WithMany(x => x.ReceiptLines)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockIssue>(entity =>
        {
            entity.ToTable("StockIssues");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DocumentNo).IsRequired().HasMaxLength(50);
            entity.HasIndex(x => x.DocumentNo).IsUnique();
            entity.Property(x => x.Customer).HasMaxLength(200);
            entity.Property(x => x.Note).HasMaxLength(500);
            entity.Property(x => x.TotalAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<StockIssueLine>(entity =>
        {
            entity.ToTable("StockIssueLines", table =>
            {
                table.HasCheckConstraint("CK_StockIssueLines_Quantity_Positive", "[Quantity] > 0");
                table.HasCheckConstraint("CK_StockIssueLines_UnitCost_NonNegative", "[UnitCost] >= 0");
            });
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UnitCost).HasPrecision(18, 2);
            entity.Property(x => x.LineTotal).HasPrecision(18, 2);
            entity.HasOne(x => x.StockIssue)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.StockIssueId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Product)
                .WithMany(x => x.IssueLines)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InventoryLedgerEntry>(entity =>
        {
            entity.ToTable("InventoryLedgerEntries");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.MovementType).IsRequired().HasMaxLength(20);
            entity.Property(x => x.ReferenceNo).IsRequired().HasMaxLength(50);
            entity.Property(x => x.UnitCost).HasPrecision(18, 2);
            entity.Property(x => x.ValueChange).HasPrecision(18, 2);
            entity.Property(x => x.RunningAverageCost).HasPrecision(18, 2);
            entity.HasIndex(x => new { x.ProductId, x.OccurredAtUtc });
            entity.HasOne(x => x.Product)
                .WithMany(x => x.LedgerEntries)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        var seededAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices", CreatedAtUtc = seededAt },
            new Category { Id = 2, Name = "Office Supplies", Description = "Consumables and office goods", CreatedAtUtc = seededAt }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Sku = "ELEC-001",
                Name = "Wireless Mouse",
                Description = "2.4GHz wireless mouse",
                CategoryId = 1,
                OnHandQty = 20,
                AverageCost = 10.00m,
                ReorderLevel = 5,
                IsActive = true,
                CreatedAtUtc = seededAt,
                LastUpdatedUtc = seededAt
            },
            new Product
            {
                Id = 2,
                Sku = "OFF-001",
                Name = "A4 Paper Pack",
                Description = "500 sheets per pack",
                CategoryId = 2,
                OnHandQty = 50,
                AverageCost = 3.00m,
                ReorderLevel = 10,
                IsActive = true,
                CreatedAtUtc = seededAt,
                LastUpdatedUtc = seededAt
            }
        );
    }
}
