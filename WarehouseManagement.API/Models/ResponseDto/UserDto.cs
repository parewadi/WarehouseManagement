using System.ComponentModel.DataAnnotations;
using WarehouseManagement.API.Models.Domain.Users;
using WarehouseManagement.API.Models.Domain;

namespace WarehouseManagement.API.Models.ResponseDto
{
    public class UserDto
    {
        
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailId { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
     }
}
