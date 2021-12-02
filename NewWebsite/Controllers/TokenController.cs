using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NewWebsite.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly InventoryContext _context;

        public TokenController(IConfiguration config, InventoryContext context)
        {
            _configuration = config;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody]UserInfo _userData)
        {

            if (_userData != null && _userData.Email != null && _userData.Password != null)
            {
                var user = await GetUser(_userData.Email, _userData.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.UserId.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("UserName", user.UserName),
                    new Claim("Email", user.Email),
                    new Claim(ClaimTypes.Role, user.RoleName)
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);
                    user.token = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(user);
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<User> GetUser(string email, string password)
        {
            try
            {
                var user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
                User userToken = new User();
                userToken.UserId = user.UserId;
                userToken.UserName = user.UserName;
                userToken.FirstName = user.FirstName;
                userToken.LastName = user.LastName;
                userToken.Email = user.Email;
                userToken.Password = user.Password;
                userToken.CreatedDate = user.CreatedDate;
                userToken.RoleName = user.RoleName;

                return userToken;
            }
            catch (Exception)
            {

                return null;
            }
            
        }
    }
}
