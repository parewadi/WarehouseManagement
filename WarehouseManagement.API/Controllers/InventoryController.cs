using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.API.Models.UnitOfWork;
using WarehouseManagement.API.Services;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SalesManager,Administrator")]
    public class InventoryController : ControllerBase
    {

        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchInventory(string? productName = null, string? warehouseName = null, string? pincode = null, string? code=null)
        {
            var result = await _inventoryService.SearchInventoryAsync(productName, warehouseName, pincode, code);

            if (result == null || !result.Any())
            {
                return NotFound("No matching inventory records found.");
            }
            return Ok(result);
        }


    }
}
