using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Statuses> Statuses { get; set; }
        public DbSet<OrderNumber> OrderNumbers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShippingCompany> ShippingCompanies { get; set; }
        public DbSet<ShippingRates> ShippingRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Product ilişkileri
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierID);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductID);

            // OrderDetails ilişkileri
            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Shipment>()
           .HasOne(s => s.Status)
           .WithMany(st => st.Shipments)
           .HasForeignKey(s => s.StatusID)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductID);

            // Order ilişkileri
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderID);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Status)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusID);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderNumber)
                .WithOne(on => on.Order)
                .HasForeignKey<Order>(o => o.OrderNumberID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingCompany)
                .WithMany() // Burada `WithMany()` yerine `WithOne()` kullanmayı düşünün
                .HasForeignKey(o => o.ShippingCompanyID);

            // Statuses ilişkileri
            modelBuilder.Entity<Statuses>()
                .HasMany(s => s.Orders)
                .WithOne(o => o.Status)
                .HasForeignKey(o => o.StatusID);

            modelBuilder.Entity<ShippingRates>()
            .HasOne(sr => sr.ShippingCompany)
            .WithMany()
            .HasForeignKey(sr => sr.ShippingCompanyID);
        }
    }
}
