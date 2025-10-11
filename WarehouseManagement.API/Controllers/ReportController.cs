using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.API.Services;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportController(IReportService reportService, IHttpContextAccessor httpContextAccessor)
        {
            _reportService = reportService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("inventory")]
        [Authorize(Roles = "Administrator,WarehouseManager")]
        public async Task<IActionResult> GetInventoryReport()
        {
            int? warehouseId = null;

           // if warehouse manager, restrict to their warehouse
            if (User.IsInRole("WarehouseManager"))
            {
                warehouseId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst("WarehouseId").Value);
            }

            var report = await _reportService.GetInventoryReportAsync(warehouseId);
            return Ok(report);
        }

        [HttpGet("orders")]
        [Authorize(Roles = "Administrator,SalesManager,WarehouseManager")]
        public async Task<IActionResult> GetOrderReport()
        {
            int? salesPersonId = null;
            int? warehouseId = null;

            if (User.IsInRole("SalesPerson"))
                salesPersonId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst("UserId").Value);

            if (User.IsInRole("WarehouseManager"))
                warehouseId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst("WarehouseId").Value);

            var report = await _reportService.GetOrderReportAsync(salesPersonId, warehouseId);
            return Ok(report);
        }
    }
}
