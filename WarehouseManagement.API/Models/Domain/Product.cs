using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.API.Models.Domain
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [MaxLength(100)]

        [Required(ErrorMessage = "Product Name is required!!")]
        public string ProductName { get; set; } = null!;
        
        [MaxLength(50)]
        [Required(ErrorMessage = "Store Keeping Unit (SKU) is required.Ex-TV-LED-43-ABC123")]
        public string SKU { get; set; } = null!;
        [MaxLength(200)]
        public string? Description { get; set; }


        //Navigation
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<Transfer>? Transfers { get; set; } = new List<Transfer>();
    }
}
