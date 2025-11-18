using System;
using Microsoft.EntityFrameworkCore;

namespace Saksupermarketsytemmvc.web.Models
{
    public partial class SaksoftSupermarketSystemContext : DbContext
    {
        public SaksoftSupermarketSystemContext() { }

        public SaksoftSupermarketSystemContext(DbContextOptions<SaksoftSupermarketSystemContext> options)
            : base(options) { }

        // DbSets
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        // NEW
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BillItem> BillItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Data Source=CDC163;Initial Catalog=SaksoftSupermarketSystem;Integrated Security=True;Trust Server Certificate=True");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CATEGORY
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.ToTable("Category");

                entity.Property(e => e.CategoryName).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Description).HasMaxLength(255).IsUnicode(false);
            });

            // CUSTOMER
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerName).HasMaxLength(150).IsUnicode(false);
                entity.Property(e => e.CustEmail).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.CustPhone).HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.CustAddress).HasMaxLength(255).IsUnicode(false);
            });

            // INVENTORY TRANSACTION
            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(e => e.TransId);
                entity.ToTable("InventoryTransaction");

                entity.Property(e => e.Remarks).HasMaxLength(255).IsUnicode(false);

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.InventoryTransactions)
                      .HasForeignKey(d => d.ProductId);
            });

            // ORDERS
            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.ToTable("orders");
            });

            // ORDER DETAILS
            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);
                entity.ToTable("OrderDetails");

                entity.HasOne(d => d.Order)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(d => d.OrderId);

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(d => d.ProductId);
            });

            // PRODUCTS
            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.ToTable("Products");
            });

            // SUPPLIER
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierId);
                entity.ToTable("Supplier");
            });

            // USER
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.ToTable("User");
            });

            // ⭐ BILL
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasKey(e => e.BillId);
                entity.ToTable("Bills");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ⭐ BILL ITEM
            modelBuilder.Entity<BillItem>(entity =>
            {
                entity.HasKey(e => e.BillItemId);
                entity.ToTable("BillItems");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.BillItems)
                    .HasForeignKey(d => d.BillId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.BillItems)
                    .HasForeignKey(d => d.ProductId);
            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
