using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Servicios.PagoSrv;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaV1.Controllers
{
    [ApiController]
    [Route("Api/[Controller]")]
    public class RegistrarPagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;
        public RegistrarPagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarVentasPagadas(
            [FromBody] ReporteClientePagoDto reporteClienteVentaDto
            , CancellationToken ct)
        {

            try
            {
                await _pagoService.CrearPagoseAsync(reporteClienteVentaDto, ct);
                return Ok(new { message = "pago registrado correctamente." });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al registrar el pago: {ex.Message}" });
            }
        }


        [HttpGet("valores-a-pagar")]
        public async Task<ActionResult<List<ValoresVentasDto>>> VerValoresAPagar(
            [FromQuery] ValoresConsultVentasDto valoresDeVentas,
            CancellationToken ct = default)
        {
            try
            {
                var resultados = await _pagoService.verValoresAPagar(valoresDeVentas, ct);
                return resultados;
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(
                    new
                    {
                        mensaje = ex.Message
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al obtener los valores a pagar: {ex.Message}" });
            }
        }
    }
}
