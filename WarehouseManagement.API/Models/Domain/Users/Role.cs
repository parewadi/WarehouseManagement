using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.API.Models.Domain.Users
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
