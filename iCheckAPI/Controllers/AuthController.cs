using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using iCheckAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace iCheckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICheckContext _context;
        private readonly IConfiguration _config;

        public AuthController(ICheckContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(Users userLogin)
        {
            var user = _context.Users.FirstOrDefault((x) => ((x.Username == userLogin.Username || x.Email == userLogin.Username) && x.Password == userLogin.Password));
            // var user = _context.Users.Where(x => (x.Username == userLogin.Username || x.Email == userLogin.Email))
            if (user == null)
                return Ok(new { message = "Username ou mot de passe incorrect" });
            //return StatusCode(StatusCodes.Status401Unauthorized, new
            //{
            //    message = "Username ou mot de passe incorrect",

            //});
            //return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var libelle = await GetRole(user.IdRole);
            var site = await GetSite(user.IdSite);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                username = user.Username,
                site = site,
                role = libelle
            });
        }

        public async Task<string> GetRole(int? id)
        {
            var role = await _context.Role.FirstOrDefaultAsync(x => x.Id == id);
            if (role == null)
                return "null";
            return role.Libelle;
        }

        public async Task<string> GetSite(int? id)
        {
            var site = await _context.Site.FirstOrDefaultAsync(x => x.Id == id);
            if (site == null)
                return "null";
            return site.Libelle;
        }
    }

}
