using Microsoft.EntityFrameworkCore.Storage;
using WarehouseManagement.API.Models.Domain;
using WarehouseManagement.API.Models.Domain.Users;
using WarehouseManagement.API.Models.Repository;

namespace WarehouseManagement.API.Models.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Warehouse> Warehouses { get; }
        IGenericRepository<Transfer> Transfers { get; }
        IGenericRepository<Inventory> Inventories { get; }
        IGenericRepository<User> Users { get; }
        IGenericRepository<Role> Roles { get; }

        IGenericRepository <UserRole>  UserRoles{ get; }

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task<int> SaveAsync();
    }
}
