namespace WarehouseManagement.API.Models.ReportDto
{
    public class OrderReportDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemReportDto> Items { get; set; }
    }
}
