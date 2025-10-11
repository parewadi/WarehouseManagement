using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.API.Models.Domain
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }
        public int Quantity { get; set; }
        
        public int WarehouseId { get; set; }
        public Warehouse  Warehouse { get; set; }
        public int  ProductId { get; set; }
        public Product Product { get; set; }
       

    }
}
