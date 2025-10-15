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
    }
}
