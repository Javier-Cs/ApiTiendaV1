using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;

namespace ApiTiendaV1.Repositorios.VentaRop
{
    public interface IVentaRepo
    {
        Task<int> CrearVenAsync(VentaCrearDto dto, CancellationToken ct = default);
        Task<VentaCompletaDto?> ObtenerVenPorIdVenAsync(int idVenta, CancellationToken ct = default);
        Task<IEnumerable<VentaDto>> ObtenerTodasLasVenAsync(CancellationToken ct = default);
        Task<bool> EliminarVentAsync(int idVenta, int idCliente, CancellationToken ct = default);

        //---- nuevos
        Task<IEnumerable<VentaDto>> ObtenerTodasVenConDeudaAsync(CancellationToken ct = default);
        Task<IEnumerable<VentaDto>> ObtenerVentasPorClienteAsync(int idCliente, CancellationToken ct = default);
        Task<IEnumerable<VentaDto>> ObtenerVenDeudaPorClienteAsync(int idCliente, string estadoVenta, string tipoVenta,  CancellationToken ct = default);
        Task<bool> ActualizarVentaAsync(int idVenta, VentaUpDto ventaAActualizar, CancellationToken ct = default);
    }
}
