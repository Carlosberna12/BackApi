using BackendApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public CuentasController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] CredencialesUsuario credenciales)
        {
            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var resultado = await userManager.CreateAsync(usuario, credenciales.Password);

            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(usuario, "Admin");
                return Ok(await ConstruirToken(credenciales));
            }

            return BadRequest(resultado.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CredencialesUsuario credenciales)
        {
            var resultado = await signInManager.PasswordSignInAsync(credenciales.Email, credenciales.Password, false, false);

            if (resultado.Succeeded)
            {
                return Ok(await ConstruirToken(credenciales));
            }

            return BadRequest("Login incorrecto");
        }

        private async Task<RespuestaLogin> ConstruirToken(CredencialesUsuario credenciales)
        {
            var claims = new List<Claim>
            {
                new Claim("email", credenciales.Email),
                new Claim(ClaimTypes.Name, credenciales.Email)
            };

            var usuario = await userManager.FindByEmailAsync(credenciales.Email);
            var roles = await userManager.GetRolesAsync(usuario);

            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddHours(4);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: null,
                claims: claims,
                expires: expiracion,
                signingCredentials: creds
            );

            return new RespuestaLogin
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await userManager.Users
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName
                }).ToListAsync(); 


            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(string id)
        {
            var usuario = await userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Email = usuario.Email,
                UserName = usuario.UserName
            };
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarUsuario(string id, [FromBody] EditarUsuarioDTO dto)
        {
            var usuario = await userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();

            usuario.UserName = dto.UserName;
            usuario.Email = dto.Email;

            var resultado = await userManager.UpdateAsync(usuario);
            if (!resultado.Succeeded) return BadRequest(resultado.Errors);

            return NoContent();
        }

 
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(string id)
        {
            var usuario = await userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();

            var resultado = await userManager.DeleteAsync(usuario);
            if (!resultado.Succeeded) return BadRequest(resultado.Errors);

            return NoContent();
        }
    }

    public class CredencialesUsuario
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RespuestaLogin
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
    }
}
