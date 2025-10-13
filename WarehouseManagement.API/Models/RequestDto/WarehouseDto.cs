namespace WarehouseManagement.API.Models.RequestDto
{
    public class WarehouseDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Pincode { get; set; } = null!;
    }

    public class StockRequestDto
    {
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class TransferDto
    {
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public string? Comments { get; set; }

        public int? OrderId { get; set; }
    }
}
