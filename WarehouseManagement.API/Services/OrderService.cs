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

        public async Task<IEnumerable<WarehouseOrderViewDto>> GetOrdersByWarehouseAsync(int warehouseId)
        {
            var orders = await _unitOfWork.Orders.GetAllAsync(
        filter: o => o.AssignedWarehouseId == warehouseId,
        includeProperties: "Items.Product,AssignedWarehouse,SalesPerson"
    );

            var allInventories = await _unitOfWork.Inventories.GetAllAsync(
                includeProperties: "Warehouse,Product"
            );

            var result = orders.Select(o => new WarehouseOrderViewDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                CustomerName = o.CustomerName ?? string.Empty,
                Status = o.Status,
                Comments = o.Notes ?? string.Empty,
                CreatedAt = o.CreatedAt,
                WarehouseName = o.AssignedWarehouse.WarehouseName,
                SalesPerson = o.SalesPerson.UserName,
                Items = o.Items.Select(i => new WarehouseOrderItemViewDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.ProductName,
                    QuantityRequested = i.QuantityRequested,
                    WarehousesWithStock = allInventories
                        .Where(inv => inv.ProductId == i.ProductId && inv.Quantity > 0)
                        .Select(inv => new WarehouseStockDto
                        {
                            WarehouseId = inv.WarehouseId,
                            WarehouseName = inv.Warehouse.WarehouseName,
                            AvailableQty = inv.Quantity
                        })
                        .ToList()
                }).ToList()
            });


            return (IEnumerable<WarehouseOrderViewDto>)result;
        }
        
        public async Task<bool> FulfillOrderAsync(int orderId, string comments)
        {
            var order = await _unitOfWork.Orders.GetAsync(
                o => o.OrderId == orderId,
                includeProperties: "Items,AssignedWarehouse"
            );

            if (order == null) return false;

            foreach (var item in order.Items)
            {
                var inventory = (await _unitOfWork.Inventories
                    .FindAsync(i => i.ProductId == item.ProductId && i.WarehouseId == order.AssignedWarehouseId))
                    .FirstOrDefault();

                if (inventory == null || inventory.Quantity < item.QuantityRequested)
                {
                    // insufficient stock — rollback gracefully
                    throw new InvalidOperationException($"Insufficient inventory for product {item.ProductId}");
                }

                inventory.Quantity -= item.QuantityRequested;
                _unitOfWork.Inventories.Update(inventory);
            }


            order.Status = "Fulfilled";
            order.Notes = AppendComment(order.Notes, $"Order fulfilled: {comments}");
            order.CreatedAt = DateTime.Now;

            await _unitOfWork.SaveAsync();
            return true;
        }

        private string AppendComment(string existing, string newComment)
        {
            var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {newComment}";
            if (string.IsNullOrWhiteSpace(existing))
                return entry;
            return existing + Environment.NewLine + entry;
        }

        public async Task<bool> CancelOrderAsync(int orderId, string comments)
        {
            var order = await _unitOfWork.Orders.GetAsync(
        o => o.OrderId == orderId,
        includeProperties: "Transfers"
    );
            if (order == null) return false;

            // revert transfers if any
            foreach (var tr in order.Transfers.Where(t => !t.IsReverted))
            {
                var fromInv = await _unitOfWork.Inventories.GetAsync(
                    i => i.WarehouseId == tr.FromWarehouseId && i.ProductId == tr.ProductId);
                var toInv = await _unitOfWork.Inventories.GetAsync(
                    i => i.WarehouseId == tr.ToWarehouseId && i.ProductId == tr.ProductId);

                if (toInv != null && toInv.Quantity >= tr.Quantity)
                    toInv.Quantity -= tr.Quantity;

                if (fromInv != null)
                    fromInv.Quantity += tr.Quantity;

                tr.IsReverted = true;
                tr.Comments += $" | Reverted on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            }

            // mark order canceled
            order.Status = "Cancelled";
            order.Notes = AppendComment(order.Notes, $"Order cancelled: {comments}");
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> TransferOrderItemAsync(int orderId, int productId, int fromWarehouseId, int toWarehouseId, int quantity, string notes)
        {
            var order = await _unitOfWork.Orders.GetAsync(
                o => o.OrderId == orderId,
                includeProperties: "AssignedWarehouse"
            );
            if (order == null) return false;

            var fromInv = await _unitOfWork.Inventories.GetAsync(
                i => i.WarehouseId == fromWarehouseId && i.ProductId == productId);
            if (fromInv == null || fromInv.Quantity < quantity)
                return false;
               // throw new InvalidOperationException("Insufficient stock in source warehouse.");

            var toInv = await _unitOfWork.Inventories.GetAsync(
                i => i.WarehouseId == toWarehouseId && i.ProductId == productId);

            // adjust stock
            fromInv.Quantity -= quantity;
            if (toInv != null)
                toInv.Quantity += quantity;
            else
                await _unitOfWork.Inventories.AddAsync(new Inventory
                {
                    WarehouseId = toWarehouseId,
                    ProductId = productId,
                    Quantity = quantity
                });

            // log transfer
            var transfer = new Transfer
            {
                OrderId = orderId,
                ProductId = productId,
                FromWarehouseId = fromWarehouseId,
                ToWarehouseId = toWarehouseId,
                Quantity = quantity,
                CreatedOn = DateTime.Now,
                Comments = notes,
                IsReverted = false
            };
            await _unitOfWork.Transfers.AddAsync(transfer);

            var product = _unitOfWork.Products.GetAsync(i => i.ProductId == transfer.ProductId);
            var FromWarehouse = _unitOfWork.Warehouses.GetAsync(o => o.WarehouseId == transfer.FromWarehouseId);
            var ToWarehouse = _unitOfWork.Warehouses.GetAsync(o => o.WarehouseId == transfer.ToWarehouseId);


            //  comment on order
            order.Notes = AppendComment(order.Notes,
                $"Transferred {quantity} of Product:{product.Result.ProductName} from Warehouse : {FromWarehouse.Result.WarehouseName} → {ToWarehouse.Result.WarehouseName}.");

            await _unitOfWork.SaveAsync();
            return true;
        }

    }

}

