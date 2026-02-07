using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.ClienteDt;

namespace ApiTiendaV1.Repositorios.ClienteRop
{
    public interface IClienteRepo
    {
        Task<int> CrearCliAsync(ClienteCrearDto dto, CancellationToken ct = default);
        Task<ClienteCompleDto?> ObtenerCliPorIdAsync(int idCliente, CancellationToken ct = default);
        Task<IEnumerable<ClienteDto>> ObtenerTodosLosCliAsync(CancellationToken ct = default);
        Task<bool> ActualizarCliAsync(int idCliente, ClienteUpDto dto, CancellationToken ct = default);
        Task<bool> EliminarCliAsync(int idCliente, CancellationToken ct = default);

        //------nuevos
        Task<IEnumerable<ClienteDto>> ObtenerTodosLosCLIEstadoAsync(bool estadoCliente, string tipocliente, CancellationToken ct = default);
        Task<IEnumerable<ClienteSearchDto>> BuscarCliAsync(string tipocliente, CancellationToken ct = default);
    }
}
