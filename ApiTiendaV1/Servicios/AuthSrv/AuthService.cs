using ApiTiendaV1.Data;
using ApiTiendaV1.DTOs.Auths;
using ApiTiendaV1.Repositorios.Auth;

namespace ApiTiendaV1.Servicios.AuthSrv
{
    public class AuthService : IAuthService
    {
        private readonly UserAuthRepo _userAuthRepo;
        private readonly ContarIntentosRepo _contarIntentosRepo;

        public AuthService(UserAuthRepo userAuthRepo, ContarIntentosRepo contarIntentosRepo)
        {
            _userAuthRepo = userAuthRepo;
            _contarIntentosRepo = contarIntentosRepo;
        }

        public async Task<SLUsuarioLoginDto> LoginAsync(LoginDto loginDto, string ip, CancellationToken ct)
        {
        }
    }
}
