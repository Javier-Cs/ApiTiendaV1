using ApiTiendaV1.DTOs;

namespace ApiTiendaV1.Repositorios.PagoRop
{
    public interface IPagoRepo
    {
        Task CrearPagoAsync(ReporteClientePagoDto dto, CancellationToken ct = default);
    }
}
