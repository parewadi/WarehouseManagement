namespace WarehouseManagement.API.Models.RequestDto
{
    public class GetProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string? Description { get; set; }
    }
}
