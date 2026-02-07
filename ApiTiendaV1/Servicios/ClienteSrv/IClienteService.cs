using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.ClienteDt;

namespace ApiTiendaV1.Servicios.ClienteSrv
{
    public interface IClienteService
    {
        Task<int> Crear_CliAsync(ClienteCrearDto dto, CancellationToken ct = default);
        Task<bool> Actualizar_CliAsync(int idCliente, ClienteUpDto dto, CancellationToken ct = default);
        Task<bool> Eliminar_CliAsync(int idCliente, CancellationToken ct = default);
        Task<ClienteCompleDto?> Obtener_CliPorIdAsync(int idCliente, CancellationToken ct = default);
        Task<IEnumerable<ClienteDto>> ObtenerTodos_LosCliAsync(CancellationToken ct = default);
        Task<IEnumerable<ClienteDto>> ObtenerTodos_LosCLIEstadoAsync(bool estadoCliente, string tipocliente, CancellationToken ct = default);
        Task<IEnumerable<ClienteSearchDto>> Buscar_CliAsync(string tipocliente, CancellationToken ct = default);
    }
}
