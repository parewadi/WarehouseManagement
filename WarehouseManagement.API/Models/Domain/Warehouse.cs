using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.API.Models.Domain
{
    public class Warehouse
    {
        [Key]
        public int WarehouseId { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage ="Warehouse Name is required!!")]
        public string WarehouseName { get; set; } = null!;
        [MaxLength(20)]
        [Required(ErrorMessage = "Warehouse Code is required!!")]
        public string Code { get; set; } = null!;
        [MaxLength(50)]
        public string Location { get; set; } = null!;
        
        [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Invalid PIN code")]
        public string Pincode { get; set; } = null!;

        // Navigation
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Transfer> TransfersFrom { get; set; } = new List<Transfer>();
        public ICollection<Transfer> TransfersTo { get; set; } = new List<Transfer>();

    }
}
