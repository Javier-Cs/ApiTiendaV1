using ApiTiendaV1.DTOs;

namespace ApiTiendaV1.Servicios.PagoSrv
{
    public interface IPagoService
    {
        public Task CrearPagoseAsync(ReporteClientePagoDto dto, CancellationToken ct = default);
    }
}
