using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.API.Migrations;
using WarehouseManagement.API.Models.Domain;
using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Models.ResponseDto;
using WarehouseManagement.API.Models.UnitOfWork;


namespace WarehouseManagement.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseService _warehouseService;
        public OrderService(IUnitOfWork unitOfWork, IWarehouseService warehouseService)
        {
            _unitOfWork = unitOfWork;
            _warehouseService = warehouseService;
        }
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();

            return orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                AssignedWarehouseId = o.AssignedWarehouseId,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    QuantityRequested = i.QuantityRequested,
                    QuantityFulfilled = i.QuantityServed
                }).ToList()
            });

        }
       
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
                AssignedWarehouseId = dto.AssignedWarehouseId,
                SalesPersonId = dto.SalesPersonId,
                CustomerName = dto.CustomerName,
                Status = "Pending",
                Notes = dto.Notes,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    QuantityRequested = i.QuantityRequested,
                    QuantityServed = 0
                }).ToList()
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveAsync();

            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                AssignedWarehouseId = order.AssignedWarehouseId,
                Notes = order.Notes,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    QuantityRequested = i.QuantityRequested,
                    QuantityFulfilled = i.QuantityServed
                }).ToList()
            };
        }
        public async Task<string> FulfillOrderAsync(FulfillOrderDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);
            if (order == null) return "Order not found.";

            foreach (var item in order.Items)
            {
                var enoughStock = await _warehouseService.CheckStockAsync(dto.WarehouseId, item.ProductId, item.QuantityRequested);
                if (!enoughStock)
                {
                    order.Status = "PendingTransfer";
                    await _unitOfWork.SaveAsync();
                    return "Not enough stock, transfer required.";
                }

                await _warehouseService.DeductStockAsync(dto.WarehouseId, item.ProductId, item.QuantityRequested);
                item.QuantityServed = item.QuantityRequested;
            }

            order.Status = "Fulfilled";
            await _unitOfWork.SaveAsync();
            return "Order fulfilled successfully.";
        }

        public async Task<string> CancelOrderAsync(CancelOrderDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);
            if (order == null) return "Order not found.";

            order.Status = "Cancelled";
            await _unitOfWork.SaveAsync();
            return $"Order cancelled. Reason: {dto.Reason}";
        }
        public async Task<string> TransferStockAsync(TransferRequestDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);
            if (order == null) return "Order not found.";

            var fromWarehouse = await _unitOfWork.Warehouses.GetByIdAsync(dto.FromWarehouseId);
            var toWarehouse = await _unitOfWork.Warehouses.GetByIdAsync(dto.ToWarehouseId);

            if (fromWarehouse == null || toWarehouse == null)
                return "Invalid warehouse(s).";

            var fromInventory = fromWarehouse.Inventories
                .FirstOrDefault(x => x.ProductId == dto.ProductId);
            var toInventory = toWarehouse.Inventories
                .FirstOrDefault(x => x.ProductId == dto.ProductId);

            if (fromInventory == null || fromInventory.Quantity < dto.Quantity)
                return "Not enough stock in source warehouse.";

            fromInventory.Quantity -= dto.Quantity;

            if (toInventory == null)
            {
                toInventory = new Inventory
                {
                    ProductId = dto.ProductId,
                    WarehouseId = dto.ToWarehouseId,
                    Quantity = dto.Quantity
                };
                toWarehouse.Inventories.Add(toInventory);
            }
            else
            {
                toInventory.Quantity += dto.Quantity;
            }

            var transfer = new Transfer
            {
                ProductId = dto.ProductId,
                FromWarehouseId = dto.FromWarehouseId,
                ToWarehouseId = dto.ToWarehouseId,
                Quantity = dto.Quantity,
                OrderId = dto.OrderId
            };

            await _unitOfWork.Transfers.AddAsync(transfer);
            await _unitOfWork.SaveAsync();

            return "Transfer completed.";
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int id)
        {
            return await _unitOfWork.Orders.GetAsync(
                o => o.OrderId == id,
                includeProperties: "Items.Product,AssignedWarehouse,SalesPerson"
            );
        }

    }

}

