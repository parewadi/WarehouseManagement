using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WarehouseManagement.API.Models.Domain.Users;
using WarehouseManagement.API.Models.RequestDto;
using WarehouseManagement.API.Models.ResponseDto;
using WarehouseManagement.API.Models.UnitOfWork;

namespace WarehouseManagement.API.Services
{
    public class AuthService:IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        public AuthService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var user = await _unitOfWork.Users
                .GetQueryable()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName
                           && u.Password == dto.Password
                           && u.IsActive);

            if (user == null) return null;

            var roles = user.UserRoles.Where(ur => ur.Role != null).Select(ur => ur.Role.RoleName).ToList();

            var token = GenerateJwtToken(user, roles);

            return new LoginResponseDto
            {
                Token = token,
                UserName = user.UserName,
                EmailId = user.EmailId,
                Roles = roles
            };
        }
        private string GenerateJwtToken(User user, List<string> roles)
        {
            var claims = new List<Claim>
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };
            if (user.WarehouseId.HasValue)
            {
                claims.Add(new Claim("warehouse_id", user.WarehouseId.Value.ToString()));
            }


            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

