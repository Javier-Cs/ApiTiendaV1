using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Modelos;
using ApiTiendaV1.Repositorios.ClienteRop;
using ApiTiendaV1.Repositorios.VentaRop;

namespace ApiTiendaV1.Servicios.VentaSrv
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepo _ventaRepo;
        private readonly IClienteRepo _clienteRepo;

        public VentaService(IVentaRepo ventaRepo, IClienteRepo clienteRepo)
        {
            _ventaRepo = ventaRepo;
            _clienteRepo = clienteRepo;
        }

        public async Task<int> Crear_VentAsync(VentaCrearDto dto, CancellationToken ct = default)
        {
            var cliente = await _clienteRepo.ObtenerCliPorIdAsync(dto.id_cliente, ct);
            if (cliente == null || !cliente.estado)
                throw new InvalidOperationException("Cliente inválido o inactivo");

            if (dto.monto_total_Venta <= 0)
                throw new ArgumentException("El monto de la venta debe ser mayor a cero");

            if (dto.tipo_venta == TipoVenta.Contado)
            {
                if (dto.efectivo_recibido < dto.monto_total_Venta)
                    throw new ArgumentException("El efectivo recibido es insuficiente");

                dto.monto_vuelto = dto.efectivo_recibido - dto.monto_total_Venta;
                dto.estado_venta = EstadoVenta.Pagado;
            }
            else if (dto.tipo_venta == TipoVenta.Credito)
            {
                dto.monto_vuelto = 0;
                dto.estado_venta = EstadoVenta.Deuda;
            }
            else
            {
                throw new ArgumentException("Tipo de venta inválido");
            }

            return await _ventaRepo.CrearVenAsync(dto, ct);
        }


        public async Task<bool> Eliminar_VentAsync(int idVenta, int idCliente, CancellationToken ct = default)
        {
            var venta = await _ventaRepo.ObtenerVenPorIdVenAsync( idVenta, ct);
            if (venta == null)
                throw new KeyNotFoundException("Venta no encontrado");
            if (venta.id_cliente != idCliente) {
                throw new InvalidOperationException("La venta no pertenece al cliente");
            }

            return await _ventaRepo.EliminarVentAsync(idVenta, idCliente, ct);
        }

        public Task<VentaCompletaDto?> Obtener_VentPorIdVentAsync(int idventa, CancellationToken ct = default)
            => _ventaRepo.ObtenerVenPorIdVenAsync(idventa, ct);

        public Task<IEnumerable<VentaDto>> Obtener_TodasLasVentAsync(CancellationToken ct = default)
            => _ventaRepo.ObtenerTodasLasVenAsync(ct);



        //---
        public Task<IEnumerable<VentaDto>> Obtener_VentDeudaPorClienteAsync(int idCliente, string estadoVenta, string tipoVenta, CancellationToken ct = default)
            => _ventaRepo.ObtenerVenDeudaPorClienteAsync(idCliente, estadoVenta, tipoVenta, ct);

        public Task<IEnumerable<VentaDto>> Obtener_TodasVentConDeudaAsync(CancellationToken ct = default)
            => _ventaRepo.ObtenerTodasVenConDeudaAsync(ct);

        public Task<IEnumerable<VentaDto>> Obtener_VentasPorClienteAsync(int idcliente, CancellationToken ct = default)
            => _ventaRepo.ObtenerVentasPorClienteAsync(idcliente, ct);
    }
}
