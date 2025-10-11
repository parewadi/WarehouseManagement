using WarehouseManagement.API.Data;
using WarehouseManagement.API.Models.Domain;
using WarehouseManagement.API.Models.Domain.Users;
using WarehouseManagement.API.Models.Repository;

namespace WarehouseManagement.API.Models.UnitOfWork
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<Order> Orders { get; }
        public IGenericRepository<OrderItem> OrderItems { get; }
        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<Warehouse> Warehouses { get; }
        public IGenericRepository<Transfer> Transfers { get; }
        public IGenericRepository<User> Users { get; }

        public IGenericRepository<Role> Roles { get; }
        public IGenericRepository<Inventory> Inventories { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Orders = new GenericRepository<Order>(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
            Products = new GenericRepository<Product>(_context);
            Warehouses = new GenericRepository<Warehouse>(_context);
            Transfers = new GenericRepository<Transfer>(_context);
            Inventories = new GenericRepository<Inventory>(_context);
            Users = new GenericRepository<User>(_context);
            Roles = new GenericRepository<Role>(_context);
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
