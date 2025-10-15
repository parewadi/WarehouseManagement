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
    [Authorize(Roles = "SalesManager")]
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

        [HttpPost("fulfill")]
        public async Task<IActionResult> Fulfill(FulfillOrderDto dto)
            => Ok(await _orderService.FulfillOrderAsync(dto));

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel(CancelOrderDto dto)
            => Ok(await _orderService.CancelOrderAsync(dto));

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequestDto dto)
            => Ok(await _orderService.TransferStockAsync(dto));


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

    }
}




