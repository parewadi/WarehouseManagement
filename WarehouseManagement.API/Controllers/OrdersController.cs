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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("Orders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        => Ok(await _orderService.GetAllOrdersAsync());

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
            => Ok(await _orderService.CreateOrderAsync(dto));

        [HttpPost("fulfill")]
        public async Task<IActionResult> Fulfill(FulfillOrderDto dto)
            => Ok(await _orderService.FulfillOrderAsync(dto));

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel(CancelOrderDto dto)
            => Ok(await _orderService.CancelOrderAsync(dto));

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequestDto dto)
            => Ok(await _orderService.TransferStockAsync(dto));
    }
}




