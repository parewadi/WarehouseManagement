using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WarehouseManagement.API.Data;
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
        private readonly AppDbContext _appDbContext;
        public AdminController(IUnitOfWork unitOfWork,AppDbContext appDbContext)
        {
            _unitOfWork = unitOfWork;
            _appDbContext = appDbContext;
        }

        [HttpGet("GetAllWarehouse")]
        [Authorize(Roles = "Administrator,SalesManager")]
        public async Task<IActionResult> GetAllWarehouse()
        {
         var warehouse =   await _unitOfWork.Warehouses. GetAllAsync();

            if (warehouse == null)
            {
                return NotFound();
            }
            else
                return Ok(warehouse);
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

        [HttpGet("GetAllUses")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _appDbContext.TblUsers
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();

            if (users == null)
            {
                return NotFound("No users found");
            }

            var result = users.Select(u => new UserDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                EmailId = u.EmailId,
                IsActive = u.IsActive,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            });
            return Ok(result);

        }
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await _unitOfWork.Roles.GetAsync(r => r.RoleId == dto. RoleId);
            if (role == null)
                return BadRequest(new { message = "Invalid role." });

            // Warehouse check only if role is Warehouse Manager
            if (role.RoleName == "WarehouseManager" && dto.WarehouseId == null)
                return BadRequest(new { message = "WarehouseId is required for Warehouse Managers." });


            // Check duplicate username or email
            var existingUser = (await _unitOfWork.Users
                .FindAsync(u => u.UserName == dto.UserName || u.EmailId == dto.EmailId))
                .FirstOrDefault();

            if (existingUser != null)
                return Conflict(new { message = "User with this username or email already exists." });

            // Start transaction
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    UserName = dto.UserName,
                    EmailId = dto.EmailId,
                    Password = dto.Password,
                    IsActive = dto.IsActive,
                    WarehouseId = role.RoleName == "WarehouseManager" ? dto.WarehouseId : null
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveAsync();


                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = dto.RoleId
                };
                await _unitOfWork.UserRoles.AddAsync(userRole);

                // For each role name, find RoleId
                //foreach (var roleName in dto.Roles)
                //{

                //    if ((await _unitOfWork.Roles.FindAsync(r => r.RoleName == roleName)).FirstOrDefault() == null)
                //    {
                //        await transaction.RollbackAsync();
                //        return BadRequest($"Role '{roleName}' not found.");
                //    }

                //    var userRole = new UserRole
                //    {
                //        UserId = user.UserId,
                //        RoleId = (await _unitOfWork.Roles.FindAsync(r => r.RoleName == roleName)).FirstOrDefault().RoleId
                //    };

                //    await _unitOfWork.UserRoles.AddAsync(userRole);
                //}

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "User created successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error while creating user", error = ex.Message });
            }

        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventoryList()
        {
            try
            {
                var inventoryList = await _unitOfWork.Inventories.GetAllAsync(includeProperties: "Warehouse,Product");
                var result = inventoryList.Select(i => new InventoryListDto
                {
                    InventoryId = i.InventoryId,
                    WarehouseName = i.Warehouse != null ? i.Warehouse.WarehouseName : "N/A",
                    ProductName = i.Product != null ? i.Product.ProductName : "N/A",
                    Quantity = i.Quantity
                }).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving inventory list", error = ex.Message });
            }


        }
        
        [HttpPost("add-inventory")]
        public async Task<IActionResult> AddInventory([FromBody] AddInventoryDto dto)
        {
            try
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
                return Ok(new { message = "Inventory updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("OrderSummary")]
        public async Task<IActionResult> GetOrderSummary()
        {
            var summary = await _unitOfWork.Orders.GetAllAsync();

            var result = summary
                .GroupBy(o => o.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToList();

            return Ok(result);
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
