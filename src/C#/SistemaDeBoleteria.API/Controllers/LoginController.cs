using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.Extensions.Logging;
using SistemaDeBoleteria.Core;
using System.Data.Common;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
namespace SistemaDeBoleteria.API.Controllers
{
    [Route("auth/[controller]")]
    [ApiController]
    [Authorize]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly string secretkey;
        public AdoDapper.Dapper db;
        public Usuario usuario1;

        public LoginController(ILogger<LoginController> logger, IConfiguration config)
        {
            _logger = logger;
            db = new AdoDapper.Dapper();
            secretkey = config.GetSection("settings").GetSection("settingskey").ToString()!;
            usuario1 = new Usuario( "asd@gmail.com", "123");
        }

        [HttpPost("/register")]
        public IActionResult Post([FromBody] Usuario usuario)
        {

            db.AuthRegistrar(usuario);
            return Created($"usuarios/{usuario.IdUsuario}", usuario);
        }
        [HttpPost("login")]
        public IActionResult PostL([FromBody] Usuario usuario)
        {
            if (usuario.Email == "asd@gmail.com" && usuario.Clave == "123")
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretkey);
                var claims = new ClaimsIdentity();

                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Email));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                string tokenCreado = tokenHandler.WriteToken(tokenConfig);
                return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado });
            }
            else
            { 
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
            }
            // var logeado = db.buscarUsuario(usuario);
            // if (logeado is null)
            //     return NotFound();

            // if (usuario.Email == logeado.Email && usuario.Clave == usuario.Clave)
            // {
            //     var keyBytes = Encoding.ASCII.GetBytes(secretkey);
            //     var claims = new ClaimsIdentity();

            //     claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Email));

            //     var tokenDescriptor = new SecurityTokenDescriptor
            //     {
            //         Subject = claims,
            //         Expires = DateTime.UtcNow.AddMinutes(5),
            //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            //     };

            //     var tokenHandler = new JwtSecurityTokenHandler();
            //     var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            //     string tokenCreado = tokenHandler.WriteToken(tokenConfig);
            //     return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado });
            // }
            // else
            // {
            //     return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
            // }
        }
        [HttpPost("/refresh")]
        public IActionResult PostR()
        {

            return Ok();
        }
        [HttpPost("/logout")]
        public IActionResult PostLg()
        {
            return Ok();
        }
        [HttpGet("/me")]
        public IActionResult GetMe([FromRoute] int id)
        {

            return Ok();
        }
        [HttpGet("/roles")]
        public IActionResult Get()
        {
            return Ok();
        }
        [HttpPost("usuarios/{usuarioID}/roles")]
        public IActionResult PostRol()
        {
            return Created();
        }

    }
}