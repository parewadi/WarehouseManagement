using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace WarehouseManagement.API.Models.Domain.Users
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required][MaxLength(50)]
        public string UserName  { get; set; }

        [Required][MaxLength(200)]
        public string Password { get; set; }
        
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string EmailId { get; set; }
        public bool IsActive     { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
