using WarehouseManagement.API.Models.Domain.Users;

namespace WarehouseManagement.API.Models.ResponseDto
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string UserName { get; set; } = null!;

        public string EmailId { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }
}
