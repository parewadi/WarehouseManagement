using System.Linq.Expressions;
using WarehouseManagement.API.Data;
using WarehouseManagement.API.Models.UnitOfWork;

namespace WarehouseManagement.API.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<object>> SearchInventoryAsync(string? productName, string? warehouseName, string? pincode,string? code);
    }
}
