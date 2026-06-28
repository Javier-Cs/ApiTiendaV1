using ApiTiendaV1.Data;
using ApiTiendaV1.DTOs.Auths;
using ApiTiendaV1.Exceptions;
using ApiTiendaV1.Repositorios.Auth;
using ApiTiendaV1.Validation;

namespace ApiTiendaV1.Servicios.AuthSrv
{
    public class AuthService : IAuthService
    {
        private readonly UserAuthRepo _userAuthRepo;
        private readonly ContarIntentosRepo _contarIntentosRepo;
        private readonly JWTService _jwtService;

        public AuthService(UserAuthRepo userAuthRepo, ContarIntentosRepo contarIntentosRepo, JWTService jWTService)
        {
            _userAuthRepo = userAuthRepo;
            _contarIntentosRepo = contarIntentosRepo;
            _jwtService = jWTService;
        }

        public async Task<SLUsuarioLoginDto> LoginAsync(LoginDto loginDto, string ip, CancellationToken ct)
        {
            Usuario respuesta = await _userAuthRepo.ObtenerUsuarioEmail(loginDto.Email, null, ct);

            if (string.IsNullOrWhiteSpace(loginDto.Email))
            {
                throw new Exception("Credenciales incorrectas.");
            }


            if (respuesta == null)
            {
                await _contarIntentosRepo.RegistraIntentosFallidos(loginDto.Email, ip);
                var intentosActuales = await _contarIntentosRepo.ContarIntentosUltimosMinutos(loginDto.Email);
                var restantes = Math.Max(0, 5 - intentosActuales);

                throw new LoginException("credenciales incorrectas.", restantes);
            }

            if (respuesta.email == null) {
                throw new Exception("El correo no puede ser nulo.");
            }

            if (respuesta.passhass == null) {
                throw new Exception("La contraseña no puede ser nula");
            }

            if (respuesta.is_deleted) {
                throw new Exception("La cuenta no puede realizar actividad.");
            }

            // contar intentos por correos
            var intentosN = await _contarIntentosRepo.ContarIntentosUltimosMinutos(loginDto.Email);

            // contar intentos por ip
            var intentosPorOIp = await _contarIntentosRepo.ContarIntentosPorIp(ip);


            if(intentosN >= 5)
            {
                throw new LoginException("Usuario bloqueador temporalmente por  1800 sg",0, true);
            }

            if (intentosPorOIp >= 15) {
                throw new LoginException("ip bloquedada temporalmente por 3600 sg", 0, true);
            }

            var pass = loginDto.Password.Trim();


            if (!BCrypt.Net.BCrypt.Verify(pass, respuesta.passhass)){
                await _contarIntentosRepo.RegistraIntentosFallidos(loginDto.Email, ip);

                var intentosActuales = await _contarIntentosRepo.ContarIntentosUltimosMinutos(loginDto.Email);

                var restantes = Math.Max(0, 5 - intentosActuales);
                throw new LoginException("Credenciales erroneas.", restantes);
            }
            else {
                await _contarIntentosRepo.LimpiarIntentos(loginDto.Email);
            }

            var expiracion = DateTime.UtcNow.AddMinutes(60);
            var token = _jwtService.GenerarToken(respuesta, expiracion);

            return new SLUsuarioLoginDto
            {
                Token = token,
                Expiracion = expiracion,
                Estado = respuesta.estado,
                idUsuario = respuesta.id_usuario,
                rol = respuesta.rol,
                Nombre = respuesta.nombre
            };
        }
    }
}
