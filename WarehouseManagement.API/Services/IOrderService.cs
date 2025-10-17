using WarehouseManagement.API.Models.Domain;
using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Models.ResponseDto;

namespace WarehouseManagement.API.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
        Task<string> FulfillOrderAsync(FulfillOrderDto dto);
        Task<string> CancelOrderAsync(CancelOrderDto dto);
        Task<string> TransferStockAsync(TransferRequestDto dto);
        Task<Order?> GetOrderWithDetailsAsync(int id);
        Task<IEnumerable<WarehouseOrderViewDto>> GetOrdersByWarehouseAsync(int warehouseId);

        Task<bool> FulfillOrderAsync(int orderId, string comments);
        Task<bool> CancelOrderAsync(int orderId, string comments);
        Task<bool> TransferOrderItemAsync(int orderId, int productId, int fromWarehouseId, int toWarehouseId, int quantity, string comments);
    }
}
