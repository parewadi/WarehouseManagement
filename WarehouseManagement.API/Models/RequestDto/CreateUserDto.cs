namespace WarehouseManagement.API.Models.RequestDto
{
    public class CreateUserDto
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;  // later: hash this
        public string EmailId { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public List<string> Roles { get; set; } = new(); // Admin assigns roles
    }
}
