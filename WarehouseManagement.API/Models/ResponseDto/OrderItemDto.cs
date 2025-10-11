namespace WarehouseManagement.API.Models.ResponseDto
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int QuantityRequested { get; set; }
        public int QuantityFulfilled { get; set; }
    }
}
