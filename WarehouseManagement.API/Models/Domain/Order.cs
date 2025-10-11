using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using WarehouseManagement.API.Models.Domain.Users;

namespace WarehouseManagement.API.Models.Domain
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [MaxLength(50)]
        public string OrderNumber { get; set; } = null!;

        [MaxLength(100)]
        public string? CustomerName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; }

        public int AssignedWarehouseId {  get; set; }
        public Warehouse AssignedWarehouse { get; set; }

        public int SalesPersonId { get; set; }
        public User SalesPerson { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<Transfer>? Transfers { get; set; } = new List<Transfer>();

    }
}
