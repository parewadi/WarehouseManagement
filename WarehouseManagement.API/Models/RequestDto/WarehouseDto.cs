namespace WarehouseManagement.API.Models.RequestDto
{
    public class WarehouseDto
    {
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
