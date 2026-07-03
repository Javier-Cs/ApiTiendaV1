using ApiTiendaV1.DTOs.ClienteDt;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Servicios.ClienteSrv;
using ApiTiendaV1.Servicios.VentaSrv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaV1.Controllers
{
    [ApiController]
    [Route("dashboard/Api/[controller]")]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }


        // POST: api/ventas
        [Authorize]
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
        [Authorize]
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


        [Authorize]
        [HttpGet("All")]
        public async Task<IActionResult> ObtenerTodaVentas(
            CancellationToken ct)
        {
            var ventas = await _ventaService.Obtener_TodasLasVentAsync( ct);
            return Ok(ventas);
        }


        [Authorize]
        [HttpGet("ConDeuda")]
        public async Task<IActionResult> ObtenerTodaVentasDeuda(
            CancellationToken ct)
        {
            var ventas = await _ventaService.Obtener_TodasVentConDeudaAsync(ct);
            return Ok(ventas);
        }



        // GET: 
        [Authorize]
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
        [Authorize]
        [HttpDelete("{idVenta:int}/cliente/{idCliente}")]
        public async Task<IActionResult> Eliminar(
            int idVenta,
            int idCliente,
            CancellationToken ct)
        {
            await _ventaService.Eliminar_VentAsync(idVenta, idCliente, ct);
            return NoContent();
        }


        [Authorize]
        [HttpPut]
        public async Task<IActionResult> ActualizarOneVent([FromQuery]int ventaId, [FromBody] VentaUpDto dto, CancellationToken ct) { 
            await _ventaService.ActualizarVentaAsync(ventaId, dto, ct);
            return Ok(dto);
        }


        [Authorize]
        [HttpPost("FechaVenta")]
        public async Task<IActionResult> BuscarPorFecha([FromBody] BuscarVenta buscarVenta, CancellationToken ct) {

            try
            {
                var resultado = await _ventaService.Obtener_Vent_por_FechaAsync(buscarVenta, ct);
                return Ok(resultado);
            }
            catch (ArgumentException ex) {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex) {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex) {
                return BadRequest(new { mensaje = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("TotalVentasClient")]
        public async Task<IActionResult> ObtenerTotalVentasPorCliente(CancellationToken ct)
        {
            var resultado = await _ventaService.Obtener_ClientesConDeudasAsync(ct);
            return Ok(resultado);
        }



        [Authorize]
        [HttpOptions]
        public IActionResult Options()
        {
            return Ok();
        }




    }
}
