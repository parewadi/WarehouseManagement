using Microsoft.EntityFrameworkCore;
using WarehouseManagement.API.Models.Domain;
using WarehouseManagement.API.Models.Domain.Users;

namespace WarehouseManagement.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Inventory> TblInventory { get; set; }
        public DbSet<Product> TblProduct { get; set; }
        public DbSet<Order> TblOrder { get; set; }
        public DbSet<OrderItem> TblItem { get; set; }
        public DbSet<Transfer> TblTransfer { get; set; }
        public DbSet<Warehouse> TblWarehouse { get; set; }
        public DbSet<User> TblUsers { get; set; }
        public DbSet<Role> TblRoles { get; set; }
        public DbSet<UserRole> TblUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product ↔ Inventory
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId);


            // Warehouse ↔ Inventory
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Warehouse)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.WarehouseId);

            // Order ↔ Warehouse
            modelBuilder.Entity<Order>()
                .HasOne(i => i.AssignedWarehouse)
                .WithMany(j => j.Orders)
                .HasForeignKey(k => k.AssignedWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order ↔ OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            // Transfer ↔ Product
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.Product)
                .WithMany(p => p.Transfers)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfer ↔ FromWarehouse
            modelBuilder.Entity<Transfer>()
               .HasOne(t => t.FromWarehouse)
               .WithMany(w => w.TransfersFrom)
               .HasForeignKey(t => t.FromWarehouseId)
               .OnDelete(DeleteBehavior.Restrict);

            // Transfer ↔ ToWarehouse
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.ToWarehouse)
               .WithMany(w => w.TransfersTo)
               .HasForeignKey(t => t.ToWarehouseId)
               .OnDelete(DeleteBehavior.Restrict);

            //Transfer ↔ Order (optional)
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.Order)
                .WithMany(o => o.Transfers)
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            //composite primary key
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            //Relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Order → SalesPerson
            modelBuilder.Entity<Order>()
                .HasOne(o => o.SalesPerson)
                .WithMany(u => u.Orders)  
                .HasForeignKey(o => o.SalesPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            //Seed Data in User / Role / UserRole tables
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1001, UserName = "Admin", Password="Admin123", EmailId = "Parveen.Rewadia@gmail.com", IsActive = true },
                new User { UserId = 1002, UserName = "SalesTeam", Password = "Sales123", EmailId = "Praveen.Rewadia@gmail.com", IsActive = true },
                new User { UserId = 1003, UserName = "WarehouseTeam", Password = "Warehouse123", EmailId = "Parveen.Rewadia@gmail.com", IsActive = true }
                );
            modelBuilder.Entity<Role>()
                .HasData(
                    new Role { RoleId = 1, RoleName = "Administrator", Description = "Administrator" },
                    new Role { RoleId = 2, RoleName = "SalesManager", Description = "Sales Manager" },
                    new Role { RoleId = 3, RoleName = "WarehouseManager", Description = "Warehouse Manager" }
                );
            modelBuilder.Entity<UserRole>()
                .HasData(
                    new UserRole { UserId = 1001, RoleId = 1 },
                    new UserRole { UserId = 1002, RoleId = 2 },
                    new UserRole { UserId = 1003, RoleId = 3 }
                    );

        }

    }
}
