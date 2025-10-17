using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.API.Models.Domain;
using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Models.ResponseDto;
using WarehouseManagement.API.Models.UnitOfWork;
using WarehouseManagement.API.Services;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SalesManager,WarehouseManager")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "SalesManager")]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null) return Unauthorized();

            int salesPersonId = int.Parse(userIdClaim);

            var orders = await _unitOfWork.Orders.GetAllAsync(
                o => o.SalesPersonId == salesPersonId,
                includeProperties: "AssignedWarehouse"
            );
            var result = orders.Select(o => new {
                o.OrderId,
                o.OrderNumber,
                o.Status,
                o.CreatedAt,
                o.CustomerName,
                WarehouseName = o.AssignedWarehouse.WarehouseName
            });

            return Ok(result);
        }


        [HttpGet("Orders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        => Ok(await _orderService.GetAllOrdersAsync());

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create([FromBody]CreateOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

           return  Ok(await _orderService.CreateOrderAsync(dto));

        }

       
        [Authorize(Roles = "SalesManager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderWithDetailsAsync(id);
            if (order == null) return NotFound();

            var result = new
            {
                order.OrderId,
                order.OrderNumber,
                order.CustomerName,
                order.Status,
                order.Notes,
                order.CreatedAt,
                order.AssignedWarehouseId,
                Items = order.Items.Select(i => new
                {
                    i.OrderItemId,
                    i.ProductId,
                    i.Product.ProductName,
                    i.QuantityRequested
                   
                })
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetails(int id, [FromBody] UpdateOrderDto dto)
        {
            if (dto == null) return BadRequest("Invalid order data.");

            var order = await _unitOfWork.Orders.GetAsync(
                o => o.OrderId == id,
                includeProperties: "Items.Product,AssignedWarehouse"
            );

            if (order == null)
                return NotFound("Order not found.");

            // ✅ Update order fields
            order.AssignedWarehouseId = dto.AssignedWarehouseId;
            order.Status = dto.Status;
            // order.Notes = dto.Notes;

            // ✅ Handle Comments with timestamp + history
            if (!string.IsNullOrWhiteSpace(dto.Notes))
            {
                var currentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var newEntry = $"[{currentTimestamp}] {dto.Notes.Trim()}";

                if (string.IsNullOrWhiteSpace(order.Notes))
                {
                    // first comment
                    order.Notes = newEntry;
                }
                else
                {
                    // append new comment on new line
                    order.Notes += Environment.NewLine + newEntry;
                }
            }

            // ✅ Update item quantities
            foreach (var itemDto in dto.Items)
            {
                var existingItem = order.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
                if (existingItem != null)
                {
                    existingItem.QuantityRequested = itemDto.Quantity;
                }
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveAsync();

            return Ok(new { message = "Order updated successfully" });
        }

        [Authorize(Roles = "WarehouseManager")]
        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetOrdersByWarehouse(int warehouseId)
        {
            var orders = await _orderService.GetOrdersByWarehouseAsync(warehouseId);
            return Ok(orders);
            //var orders = await _orderService.GetOrdersByWarehouseAsync(warehouseId);

            //var result = orders.Select(o => new
            //{
            //    o.OrderId,
            //    o.OrderNumber,
            //    o.CustomerName,
            //    o.Status,
            //    o.CreatedAt,
            //    o.Notes,
            //    Warehouse = o.WarehouseName,
            //    SalesPerson = o.SalesPerson,
            //    Items = o.Items.Select(i => new
            //    {
            //        i.ProductId,

            //        i.QuantityRequested
            //    })
            //});

            //return Ok(result);
        }

        [Authorize(Roles = "WarehouseManager")]
        [HttpPut("fulfill/{orderId}")]
        public async Task<IActionResult> FulfillOrder(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var success = await _orderService.FulfillOrderAsync(orderId, dto.Comments);
            if (!success)
                return BadRequest(new { message = "Unable to fulfill order." });
            return Ok(new { message = "Order fulfilled successfully." });
        }

        [Authorize(Roles = "WarehouseManager")]
        [HttpPut("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var success = await _orderService.CancelOrderAsync(orderId, dto.Comments);
            if (!success)
                return BadRequest(new { message = "Unable to cancel order." });

            return Ok(new { message = "Order cancelled successfully." });

        }

        [Authorize(Roles = "WarehouseManager")]
        [HttpPost("transfer")]
        public async Task<IActionResult> TransferOrderItem([FromBody] TransferRequestDto dto)
        {
            var success = await _orderService.TransferOrderItemAsync(
                dto.OrderId, dto.ProductId, dto.FromWarehouseId, dto.ToWarehouseId, dto.Quantity, dto.Comments
            );
            if (!success) return NotFound("Order or product not found");
            return Ok(new { message = "Transfer completed successfully." });
        }

    }
}




