using JWTApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();

        public readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("protectedWithRole"), Authorize(Roles = "Admin")]
        public ActionResult<string> protectedWithRole()
        {
            return Ok("Eres un usuario autorizado y con rol");
        }


        [HttpGet("protectedAuth"), Authorize]
        public ActionResult<string> protectedAuth()
        {
            return Ok("Eres un usuario autenticado");
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserDto requestUser)
        {
            string passwordHash
                = BCrypt.Net.BCrypt.HashPassword(requestUser.Password);

            user.UserName = requestUser.Username;
            user.Rol = requestUser.Rol;
            user.PasswordHash = passwordHash;

            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<User> Login(UserDto requestUser)
        {
            if (user.UserName != requestUser.Username)
                return BadRequest("User not found");


            if (!BCrypt.Net.BCrypt.Verify(requestUser.Password, user.PasswordHash))
                return BadRequest("Wrong password");

            string token = createToken(user);

            return Ok(token);
        }

        private string createToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JWTSettings:TokenKey"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role,user.Rol)
            };

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }



}
