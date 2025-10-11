using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Models.ResponseDto;

namespace WarehouseManagement.API.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    }
}
