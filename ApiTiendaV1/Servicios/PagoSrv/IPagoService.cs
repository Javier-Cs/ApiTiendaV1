using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.DeudasPorPagarDto;
using ApiTiendaV1.DTOs.VentaDt;

namespace ApiTiendaV1.Servicios.PagoSrv
{
    public interface IPagoService
    {
        public Task CrearPagoseAsync(ReporteClientePagoDto dto, CancellationToken ct = default);
        public Task PagarDeudasVenta(VentasAPagarConDeudaDto ventasAPagarConDeudaDto, CancellationToken ct=default);
    }
}
