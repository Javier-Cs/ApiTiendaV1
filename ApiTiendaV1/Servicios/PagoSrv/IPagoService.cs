using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;

namespace ApiTiendaV1.Servicios.PagoSrv
{
    public interface IPagoService
    {
        public Task CrearPagoseAsync(ReporteClientePagoDto dto, CancellationToken ct = default);
        public Task<List<ValoresVentasDto>> verValoresAPagar(ValoresConsultVentasDto valoresDeVentas, CancellationToken ct = default);
    }
}
