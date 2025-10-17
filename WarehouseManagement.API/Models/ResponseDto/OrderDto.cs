namespace WarehouseManagement.API.Models.ResponseDto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public string? CustomerName { get; set; }
        public int AssignedWarehouseId { get; set; }

        public string? Notes { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class FulfillOrderDto
    {
        public int OrderId { get; set; }
        public int WarehouseId { get; set; }
    }

    public class CancelOrderDto
    {
        public int OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class TransferRequestDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public string Comments { get; set; } = string.Empty;
    }

    public class UpdateOrderDto
    {
        public int AssignedWarehouseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public List<OrderItemUpdateDto> Items { get; set; } = new();
    }

    public class OrderItemUpdateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
