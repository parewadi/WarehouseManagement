using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.API.Models.Domain
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public int QuantityRequested { get; set; } 
        public int QuantityServed { get; set; } = 0;

        //Foreign Keys
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }  
        public Product Product { get; set; } = null!;

       

        
    }
}
