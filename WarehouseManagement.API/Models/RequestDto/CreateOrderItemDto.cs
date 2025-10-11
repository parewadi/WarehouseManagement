namespace WarehouseManagement.API.Models.RequestDto
{
    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int QuantityRequested { get; set; }
    }
}
