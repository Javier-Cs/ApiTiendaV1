using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Repositorios.PagoRop;

namespace ApiTiendaV1.Servicios.PagoSrv
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepo  _pagoRepo;
        public PagoService(IPagoRepo pagoRepo)
        {
            _pagoRepo = pagoRepo;
        }
        public Task CrearPagoseAsync (ReporteClientePagoDto dto, CancellationToken ct = default)
        {
            if (dto == null || !dto.lista_id_vents.Any()) {
                throw new ArgumentNullException("Elemento vacio o con formato null");            
            }
            if (dto.efectivo_recibido < dto.monto_total_Venta) { 
                throw new Exception("El efectivo recibido es menor al monto total de la venta");
            }
            return _pagoRepo.CrearPagoAsync(dto, ct);


        }

        public Task<List<ValoresVentasDto>> verValoresAPagar(ValoresConsultVentasDto valoresDeVentas, CancellationToken ct = default)
        {
            if (valoresDeVentas == null || valoresDeVentas.id_venta == 0) {
                throw new ArgumentNullException("Elemento vacio o con formato null");
            }
            return _pagoRepo.ObtenerValoresVentasAsync(valoresDeVentas, ct);
        }
    }
}
