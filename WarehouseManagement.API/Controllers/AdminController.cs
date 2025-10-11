using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.API.Models.Domain;
using WarehouseManagement.API.Models.Domain.Users;
using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Models.ResponseDto;
using WarehouseManagement.API.Models.UnitOfWork;

namespace WarehouseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("create-warehouse")]
        public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseDto dto)
        {
            var warehouse = new Warehouse
            {
                WarehouseName = dto.WarehouseName,
                Code = dto.Code,
                Location = dto.Location,
                Pincode = dto.Pincode,
            };

            await _unitOfWork.Warehouses.AddAsync(warehouse);
            await _unitOfWork.SaveAsync();
            return Ok(warehouse);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Password = dto.Password,
                EmailId = dto.EmailId,
                IsActive = dto.IsActive
            };

            foreach (var roleId in dto.UserRoleID)
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);

                user.UserRoles.Add(new UserRole
                {
                    RoleId = roleId,
                    User = user,
                    Role = role
                });
            }

            await _unitOfWork.Users. AddAsync(user);
            await _unitOfWork.SaveAsync();

            return Ok(new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                EmailId = user.EmailId,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            });
        }

        [HttpPost("add-inventory")]
        public async Task<IActionResult> AddInventory([FromBody] AddInventoryDto dto)
        {
            // check if inventory already exists
            var existing = (await _unitOfWork.Inventories
                .FindAsync(i => i.WarehouseId == dto.WarehouseId && i.ProductId == dto.ProductId))
                .FirstOrDefault();

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                _unitOfWork.Inventories.Update(existing);
            }
            else
            {
                var inventory = new Inventory
                {
                    WarehouseId = dto.WarehouseId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                await _unitOfWork.Inventories.AddAsync(inventory);
            }

            await _unitOfWork.SaveAsync();
            return Ok("Inventory updated successfully");
        }

        [HttpPost("create-product")]
        [Authorize(Roles = "Administrator")] // ✅ Admin only
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                SKU = dto.SKU,
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveAsync();

            // return as DTO (avoid cycles)
            return Ok(new
            {
                product.ProductId,
                product.ProductName,
                product.Description,
                product.SKU,
              
            });
        }
    }
}
