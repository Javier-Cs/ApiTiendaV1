using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.DeudasPorPagarDto;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Repositorios.PagoRop;
using Microsoft.IdentityModel.Tokens;

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

        public Task PagarDeudasVenta(VentasAPagarConDeudaDto dto, CancellationToken ct = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.cliente == null)
                throw new Exception("cliente null");

            if (dto.lista_ventas == null || !dto.lista_ventas.Any())
                throw new Exception("lista ventas vacia");

            if (string.IsNullOrWhiteSpace(dto.cliente.telefono))
                throw new Exception("telefono requerido");

            if (string.IsNullOrWhiteSpace(dto.cliente.email))
                throw new Exception("email requerido");

            /*if (dto.efectivo_recibido < dto.monto_total_Venta)
                throw new Exception(
                    "efectivo insuficiente");*/

            //dto.vuelto = dto.efectivo_recibido - dto.monto_total_Venta;
            return _pagoRepo.PagarDeudas(dto, ct);

        }

      
    }
}
