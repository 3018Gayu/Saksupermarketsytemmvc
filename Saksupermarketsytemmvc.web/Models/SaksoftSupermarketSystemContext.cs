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
        public virtual DbSet<Orders> Orders { get; set; }           // <-- FIXED: singular
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Products> Products { get; set; }       // <-- FIXED: singular
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning Move connection string out of source code for security
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

                entity.Property(e => e.CustAddress).HasMaxLength(255).IsUnicode(false).HasColumnName("CUST_Address");
                entity.Property(e => e.CustEmail).HasMaxLength(100).IsUnicode(false).HasColumnName("CUST_Email");
                entity.Property(e => e.CustPhone).HasMaxLength(10).IsUnicode(false).HasColumnName("CUST_Phone");
                entity.Property(e => e.CustomerName).HasMaxLength(150).IsUnicode(false).HasColumnName("CUSTOMER_Name");
            });

            // INVENTORY TRANSACTION
            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.HasKey(e => e.TransId);
                entity.ToTable("InventoryTransaction");

                entity.Property(e => e.Remarks).HasMaxLength(255).IsUnicode(false);
                entity.Property(e => e.TransDate).HasColumnName("Trans_Date").HasColumnType("datetime");
                entity.Property(e => e.TransType).HasMaxLength(10).IsUnicode(false).HasColumnName("Trans_Type");

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.InventoryTransactions)
                      .HasForeignKey(d => d.ProductId)
                      .HasConstraintName("FK_Inventory_Product");
            });

            // ORDERS
            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.ToTable("orders");   // <-- FIXED: matches actual DB table name

                entity.HasIndex(e => e.InvoiceNo).IsUnique();

                entity.Property(e => e.InvoiceNo).HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.NetAmount).HasColumnType("decimal(10,2)");

                entity.HasOne(d => d.Customer)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(d => d.CustomerId);
            });

            // ORDER DETAIL
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);
                entity.ToTable("OrderDetail");

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(10,2)");

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

                entity.Property(e => e.Name).HasMaxLength(150).IsUnicode(false);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.Products)
                      .HasForeignKey(d => d.CategoryId)
                      .HasConstraintName("FK_Products_Category");
            });

            // SUPPLIER
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierId);
                entity.ToTable("Supplier");

                entity.Property(e => e.Address).HasMaxLength(255).IsUnicode(false);
                entity.Property(e => e.Contact).HasMaxLength(10).IsUnicode(false);
                entity.Property(e => e.Name).HasMaxLength(100).IsUnicode(false);
            });

            // USER
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.ToTable("User");

                entity.Property(e => e.UserName).HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.UserEmail).HasMaxLength(100).IsUnicode(false).HasColumnName("User_Email");
                entity.Property(e => e.UserRole).HasMaxLength(20).IsUnicode(false).HasColumnName("User_Role");
                entity.Property(e => e.PasswordHash).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Isactive).HasMaxLength(10).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
