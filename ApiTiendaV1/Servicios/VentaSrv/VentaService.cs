using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Modelos;
using ApiTiendaV1.Repositorios.ClienteRop;
using ApiTiendaV1.Repositorios.VentaRop;
using ApiTiendaV1.Servicios.ClienteSrv;

namespace ApiTiendaV1.Servicios.VentaSrv
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepo _ventaRepo;
        private readonly IClienteRepo _clienteRepo;
        private readonly IClienteService _clienteService;

        public VentaService(IVentaRepo ventaRepo, IClienteRepo clienteRepo, IClienteService clienteServ)
        {
            _ventaRepo = ventaRepo;
            _clienteRepo = clienteRepo;
            _clienteService = clienteServ;
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
                //dto.estado_venta = EstadoVenta.Pagado;
            }
            else if (dto.tipo_venta == TipoVenta.Contado && dto.efectivo_recibido == 0) {
                dto.monto_vuelto = 0;
            }
            else if (dto.tipo_venta == TipoVenta.Credito)
            {
                dto.efectivo_recibido = dto.monto_total_Venta;
                dto.monto_vuelto = 0;
                //dto.estado_venta = EstadoVenta.Deuda;
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

        public async Task<bool> ActualizarVentaAsync(int idVenta, VentaUpDto dto, CancellationToken ct = default)
        {
            var venta = await _ventaRepo.ObtenerVenPorIdVenAsync(idVenta, ct);
            if (venta == null) {
                throw new KeyNotFoundException("venta no encontrada");
            }
            if (dto.descripcion_venta == null && dto.tipo_venta == null && dto.efectivo_recibido == null
                && dto.monto_total_Venta == null ) 
            {
                throw new ArgumentException("No hay datos para actualizar..");
            }

            if (dto.monto_total_Venta.HasValue && dto.monto_total_Venta < 0)
                throw new ArgumentException("El monto total no puede ser negativo.");

            if (dto.efectivo_recibido.HasValue && dto.efectivo_recibido < 0)
                throw new ArgumentException("El efectivo recibido no puede ser negativo.");

            //if (dto.monto_vuelto < 0)
            //    throw new ArgumentException("El vuelto no puede ser negativo.");

            if (dto.efectivo_recibido.HasValue && dto.monto_total_Venta.HasValue && 
                dto.efectivo_recibido < dto.monto_total_Venta)
                throw new InvalidOperationException("El efectivo recibido es insuficiente.");

            //dto.monto_vuelto = dto.efectivo_recibido - dto.monto_total_Venta;

            return await _ventaRepo.ActualizarVentaAsync(idVenta, dto, ct);
            
        }

        public async Task<IEnumerable<VentaDto>> Obtener_Vent_por_FechaAsync(BuscarVenta buscarVenta, CancellationToken ct = default)
        {
            if (buscarVenta.fecha_venta == null)
                throw new ArgumentException("fecha_venta es obligatoria");
            if (buscarVenta.id_cliente == 0)
            {
                return await _ventaRepo.Obtener_Ven_por_FechaAsync(buscarVenta, ct);
            }

            var cliente = await _clienteService.Obtener_CliPorIdAsync(buscarVenta.id_cliente, ct);

            return await _ventaRepo.Obtener_Ven_por_FechaAsync(buscarVenta, ct);

        }
    }

}