using ApiTiendaV1.DTOs.Auths;
using ApiTiendaV1.Exceptions;
using ApiTiendaV1.Servicios.AuthSrv;
using ApiTiendaV1.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiTiendaV1.Controllers
{

    [ApiController]
    [Route("Api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JWTService _jWTService;
        private readonly IAuthService _authService;


        public AuthController(JWTService jWTService, IAuthService authService) {
            _jWTService = jWTService;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me() {
            return Ok(new {
                IdUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Nombre = User.FindFirst(ClaimTypes.Name)?.Value,
                Rol = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }


        [Authorize]
        [HttpGet("hash")]
        public IActionResult GenerarHash(string pass) {
            return Ok(new {
                Hash = BCrypt.Net.BCrypt.HashPassword(pass)
            });
        }


        [HttpPost("login")]
        public async Task<ActionResult<SLUsuarioLoginDto>> LoginUser([FromBody] LoginDto loginDto, CancellationToken ct) {
            try
            {
                if (loginDto == null) {
                    return BadRequest("Error, sin credenciales ingresadas.");
                }
                if (string.IsNullOrWhiteSpace(loginDto.Email)) {
                    return BadRequest("El correo es necesario");
                }
                if (string.IsNullOrWhiteSpace(loginDto.Password)) {
                    return BadRequest("La Contraseña es necesaria");
                }

                string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();

                var usuario = await _authService.LoginAsync(loginDto, ip ?? "IP_NO_DISPONIBEL", ct);

                Response.Cookies.Append(
                    "access_token",
                    usuario.Token,
                    new CookieOptions {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = usuario.Expiracion
                    }
                );

                return Ok(
                    new {
                        usuario.idUsuario,
                        usuario.Nombre,
                        usuario.rol
                    }
                );

                

            }
            catch (LoginException ex) {
                return BadRequest(new
                {
                    message = ex.Message,
                    intentosRestantes = ex.IntentosRestantes,
                    bloqueado = ex.Bloqueado
                });
            }
        }
    }
}
