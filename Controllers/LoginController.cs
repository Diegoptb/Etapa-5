using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.DTOs;
using BankAPI.Data.BankModels;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly LoginService loginService;
        private IConfiguration config;

        public LoginController(LoginService loginService, IConfiguration config)
        {
            this.loginService = loginService;
            this.config = config;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login(AdminDto adminDto)
        {
            var admin = await loginService.GetAdmin(adminDto);
            if (admin is null)
            {
                return BadRequest(new { message = "Credenciales incorrectas" });
            }
            string jwtToken = GenerateToken(admin);
            return Ok(new { token = jwtToken });
        }

        [HttpPost("authenticate/client")]
        public async Task<IActionResult> AuthenticateClient(ClientDto clientDto)
        {
            var client = await loginService.GetClientByEmailAndPassword(clientDto.Email, clientDto.Pwd);
            if (client == null)
            {
                return BadRequest(new { message = "Credenciales incorrectas" });
            }

            string jwtToken = GenerateToken(client);
            return Ok(new { token = jwtToken });
        }

        private string GenerateToken(Administrator admin)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, admin.Name),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim("AdminType", admin.AdminType)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }

        private string GenerateToken(Client client)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
                new Claim(ClaimTypes.Name, client.Name),
                new Claim(ClaimTypes.Email, client.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}