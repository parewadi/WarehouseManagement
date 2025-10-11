namespace WarehouseManagement.API.Models.RequestDto
{
    public class CreateProductDto
    {
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string? Description { get; set; }
    }
}
