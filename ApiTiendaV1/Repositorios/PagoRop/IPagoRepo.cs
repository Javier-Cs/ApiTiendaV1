using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;

namespace ApiTiendaV1.Repositorios.PagoRop
{
    public interface IPagoRepo
    {
        Task CrearPagoAsync(ReporteClientePagoDto dto, CancellationToken ct = default);
        Task<List<ValoresVentasDto>> ObtenerValoresVentasAsync(ValoresConsultVentasDto valores, CancellationToken ct = default);
    }
}
