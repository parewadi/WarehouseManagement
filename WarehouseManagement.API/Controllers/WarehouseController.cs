using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Models.UnitOfWork;
using WarehouseManagement.API.Services;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IUnitOfWork _unitOfWork;


        public WarehouseController(IWarehouseService warehouseService,IUnitOfWork unitOfWork)
        {
            _warehouseService = warehouseService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAllWarehouse")]
        public async Task<IActionResult> GetAllWarehouse()
        {
            var warehouse = await _unitOfWork.Warehouses.GetAllAsync();

            if (warehouse == null)
            {
                return NotFound();
            }
            else
                return Ok(warehouse);
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

        [HttpGet("warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetInventoryByWarehouseId(int warehouseId)
        {
            var result = await _warehouseService.GetInventoryByWarehouse(warehouseId);
            if(result == null)
                return NotFound();

            return Ok(result);
        }
        }
}
