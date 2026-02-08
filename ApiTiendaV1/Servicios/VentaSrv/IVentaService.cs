using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;

namespace ApiTiendaV1.Servicios.VentaSrv
{
    public interface IVentaService
    {
        Task<int> Crear_VentAsync(VentaCrearDto dto, CancellationToken ct = default);
        Task<bool> Eliminar_VentAsync(int idVenta, int idCliente, CancellationToken ct = default);
        Task<VentaCompletaDto?> Obtener_VentPorIdVentAsync(int idventa, CancellationToken ct = default);
        Task<IEnumerable<VentaDto>> Obtener_TodasLasVentAsync(CancellationToken ct = default);

        //---
        Task<IEnumerable<VentaDto>> Obtener_TodasVentConDeudaAsync(CancellationToken ct = default);
        Task<IEnumerable<VentaDto>> Obtener_VentasPorClienteAsync(int idcliente, CancellationToken ct = default);
        Task<IEnumerable<VentaDto>> Obtener_VentDeudaPorClienteAsync(int idCliente, string estadoVenta, string tipoVenta, CancellationToken ct = default);
        Task<bool> ActualizarVentaAsync(int idVenta, VentaUpDto dto , CancellationToken ct = default);
    }
}
