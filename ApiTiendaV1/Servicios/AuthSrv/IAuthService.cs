using ApiTiendaV1.DTOs.Auths;

namespace ApiTiendaV1.Servicios.AuthSrv
{
    public interface IAuthService
    {
        public Task<SLUsuarioLoginDto> LoginAsync(LoginDto loginDto, string ip, CancellationToken ct);
    }
}
