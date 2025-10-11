using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Services;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // GET api/warehouse/check-stock?warehouseId=..&productId=..&qty=..
        [HttpGet("check-stock")]
        public async Task<IActionResult> CheckStock(int warehouseId, int productId, int qty)
        {
            var available = await _warehouseService.CheckStockAsync(warehouseId, productId, qty);
            return Ok(new { WarehouseId = warehouseId, ProductId = productId, RequiredQty = qty, Available = available });
        }

        // POST api/warehouse/add-stock
        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock([FromBody] StockRequestDto dto)
        {
            var success = await _warehouseService.AddStockAsync(dto.WarehouseId, dto.ProductId, dto.Quantity);
            return Ok(success ? "Stock added successfully." : "Failed to add stock.");
        }

        // POST api/warehouse/deduct-stock
        [HttpPost("deduct-stock")]
        public async Task<IActionResult> DeductStock([FromBody] StockRequestDto dto)
        {
            var success = await _warehouseService.DeductStockAsync(dto.WarehouseId, dto.ProductId, dto.Quantity);
            return Ok(success ? "Stock deducted successfully." : "Not enough stock.");
        }

        // POST api/warehouse/transfer
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            var result = await _warehouseService.TransferStockAsync(dto);
            return Ok(result);
        }
    }
}
