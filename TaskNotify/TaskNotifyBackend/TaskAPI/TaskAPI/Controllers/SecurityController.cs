using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class SecurityController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SecurityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserModel loginUserModel)
        {
            var connectionString = _configuration["MSSQLDb:ConnectionString"];

            LoginUserModel? userFromDb;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("p_username", loginUserModel.UserName, DbType.String, ParameterDirection.Input);
                userFromDb = await connection.QuerySingleOrDefaultAsync<LoginUserModel>("get_user_by_name", parameters, commandType: System.Data.CommandType.StoredProcedure);
            }

            // should probably not expose if user is found or not, just return invalid username or pw for production...
            if (userFromDb is null)
            {
                return Unauthorized($"user {loginUserModel.UserName} is not found");
            }

            if (!BCrypt.Net.BCrypt.EnhancedVerify(loginUserModel.Password, userFromDb.Password))
            {
                return Unauthorized("invalid username or password !");
            }

            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, "sub"),
                new(JwtRegisteredClaimNames.Email, "email"),
                new("UserName", loginUserModel.UserName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"]))),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);
            return Ok(new {Token= jwt });
        }
    }
}
