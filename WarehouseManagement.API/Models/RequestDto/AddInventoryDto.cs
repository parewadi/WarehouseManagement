namespace WarehouseManagement.API.Models.RequestDto
{
    public class AddInventoryDto
    {
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
