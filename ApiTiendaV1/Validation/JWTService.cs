using ApiTiendaV1.DTOs.Auths;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiTiendaV1.Validation
{
    public class JWTService
    {

        IConfiguration _config;

        public JWTService(IConfiguration config) {
            _config = config;
        }

        // generar Token

        public string GenerarToken(Usuario user, DateTime expiracion) {

            //datos que vas dentro del token
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, user.id_usuario.ToString()),
                new Claim(ClaimTypes.Name, user.nombre),
                new Claim(ClaimTypes.Role, user.rol),
                new Claim(ClaimTypes.Email, user.email),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                ),

                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64
                )
            };

            //crear la llave
            var key = new SymmetricSecurityKey(
                // convierte a la llave en bytes de los datos de appsettings.json
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );
            // crea las credenciales de la llave y se firma
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // crea el token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiracion,
                signingCredentials: cred
            );

            // se escribe el token y se retorma en texto
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
