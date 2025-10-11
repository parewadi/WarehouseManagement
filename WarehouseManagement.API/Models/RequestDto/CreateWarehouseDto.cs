namespace WarehouseManagement.API.Models.RequestDto
{
    public class CreateWarehouseDto
    {
        public string WarehouseName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Pincode { get; set; } = null!;

    }
}
