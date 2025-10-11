using Microsoft.EntityFrameworkCore;
using WarehouseManagement.API.Data;
using WarehouseManagement.API.Models.Domain;
using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Models.UnitOfWork;

namespace WarehouseManagement.API.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseService(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CheckStockAsync(int warehouseId, int productId, int requiredQty)
        {
            var stock = await _context.TblInventory
                .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId && i.ProductId == productId);

            return stock != null && stock.Quantity >= requiredQty;
        }

        public async Task<bool> DeductStockAsync(int warehouseId, int productId, int qty)
        {
            var stock = await _context.TblInventory
                .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId && i.ProductId == productId);

            if (stock == null || stock.Quantity < qty)
                return false;

            stock.Quantity -= qty;
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> AddStockAsync(int warehouseId, int productId, int qty)
        {
            var stock = await _context.TblInventory
                .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId && i.ProductId == productId);

            if (stock == null)
            {
                stock = new Inventory
                {
                    WarehouseId = warehouseId,
                    ProductId = productId,
                    Quantity = qty
                };
                await _context.TblInventory.AddAsync(stock);
            }
            else
            {
                stock.Quantity += qty;
            }

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<string> TransferStockAsync(TransferDto transferDto)
        {
            // Deduct from source
            var fromStock = await _context.TblInventory
                .FirstOrDefaultAsync(i => i.WarehouseId == transferDto.FromWarehouseId && i.ProductId == transferDto.ProductId);

            if (fromStock == null || fromStock.Quantity < transferDto.Quantity)
                return "Not enough stock in source warehouse.";

            fromStock.Quantity -= transferDto.Quantity;

            // Add to target
            var toStock = await _context.TblInventory
                .FirstOrDefaultAsync(i => i.WarehouseId == transferDto.FromWarehouseId && i.ProductId == transferDto.ProductId);


            if (toStock == null)
            {
                toStock = new Inventory
                {
                    WarehouseId = transferDto.ToWarehouseId,
                    ProductId = transferDto.ProductId,
                    Quantity = transferDto.Quantity
                };
                await _context.TblInventory.AddAsync(toStock);
            }
            else
            {
                toStock.Quantity += transferDto.Quantity;
            }

            // Record transfer
            var transfer = new Transfer
            {
                ProductId = transferDto.ProductId,
                FromWarehouseId = transferDto.FromWarehouseId,
                ToWarehouseId = transferDto.ToWarehouseId,
                Quantity = transferDto.Quantity,
                OrderId = transferDto.OrderId,
                Comments = transferDto.Comments,
            };
            await _unitOfWork.Transfers.AddAsync(transfer);

            await _unitOfWork.SaveAsync();
            return "Transfer completed.";
        }
    }

}
