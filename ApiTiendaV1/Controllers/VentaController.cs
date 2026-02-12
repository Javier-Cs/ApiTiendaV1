using ApiTiendaV1.DTOs.ClienteDt;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Servicios.ClienteSrv;
using ApiTiendaV1.Servicios.VentaSrv;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaV1.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] VentaCrearDto dto,
            CancellationToken ct)
        {
            var id = await _ventaService.Crear_VentAsync(dto, ct);
            return CreatedAtAction(nameof(ObtenerPorId),
                new { idVenta = id, idCliente = dto.id_cliente },
                new { id });
        }

        // GET: 
        [HttpGet("cliente/{idCliente:int}/estado/{estadoVenta}/tipo/{tipoVenta}")]
        public async Task<IActionResult> ObtenerPorCliente(
            int idCliente,
            string estadoVenta,
            string tipoVenta,
            CancellationToken ct)
        {
            var ventas = await _ventaService.Obtener_VentDeudaPorClienteAsync (idCliente,estadoVenta, tipoVenta, ct);
            return Ok(ventas);
        }

        [HttpGet("All")]
        public async Task<IActionResult> ObtenerTodaVentas(
            CancellationToken ct)
        {
            var ventas = await _ventaService.Obtener_TodasLasVentAsync( ct);
            return Ok(ventas);
        }


        [HttpGet("ConDeuda")]
        public async Task<IActionResult> ObtenerTodaVentasDeuda(
            CancellationToken ct)
        {
            var ventas = await _ventaService.Obtener_TodasVentConDeudaAsync(ct);
            return Ok(ventas);
        }



        // GET: 
        [HttpGet("{idVenta:int}")]
        public async Task<IActionResult> ObtenerPorId(
            int idVenta,
            CancellationToken ct)
        {
            var venta = await _ventaService.Obtener_VentPorIdVentAsync(idVenta, ct);
            if (venta == null)
                return NotFound();

            return Ok(venta);
        }

        // DELETE:  
        [HttpDelete("{idVenta:int}/cliente/{idCliente}")]
        public async Task<IActionResult> Eliminar(
            int idVenta,
            int idCliente,
            CancellationToken ct)
        {
            await _ventaService.Eliminar_VentAsync(idVenta, idCliente, ct);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarOneVent([FromQuery]int ventaId, [FromBody] VentaUpDto dto, CancellationToken ct) { 
            await _ventaService.ActualizarVentaAsync(ventaId, dto, ct);
            return Ok(dto);
        }



        [HttpOptions]
        public IActionResult Options()
        {
            return Ok();
        }
    }
}
