using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.API.Data;
using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Models.UnitOfWork;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _appDbContext;
        public ProductController(AppDbContext appDbContext,IUnitOfWork unitOfWork)
        {
            this._appDbContext = appDbContext;
            this._unitOfWork = unitOfWork;
        }

        [HttpGet("Products")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            if (products == null)
                return NotFound("Product not Found");

            var produdtDto = products.Select(p => new CreateProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                SKU = p.SKU,
                Description = p.Description
            });

            return Ok(produdtDto);
        }
    }
}
