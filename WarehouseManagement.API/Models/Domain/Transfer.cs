using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace WarehouseManagement.API.Models.Domain
{
    public class Transfer
    {
        [Key]
        public int TransferId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [MaxLength(500)]
        public string? Comments { get; set; }

        public bool IsReverted { get; set; } = false;

        //Foreign Keys
        public int? ProductId { get; set; }
        public Product? Product { get; set; } = null!;
        public int FromWarehouseId { get; set; }
        public Warehouse FromWarehouse { get; set; }
        public int ToWarehouseId { get; set; }
        public Warehouse ToWarehouse { get; set; }
       
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
