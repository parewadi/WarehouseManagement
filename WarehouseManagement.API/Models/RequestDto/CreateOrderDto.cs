using WarehouseManagement.API.Models.ResponseDto;

namespace WarehouseManagement.API.Models.RequestDto
{
    public class CreateOrderDto
    {
        public string OrderNumber { get; set; } = null!;
        public int AssignedWarehouseId { get; set; }
        public int SalesPersonId { get; set; }   // NEW
        public string? CustomerName { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
