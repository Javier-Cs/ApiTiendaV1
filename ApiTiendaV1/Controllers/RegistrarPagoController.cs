using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.DeudasPorPagarDto;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Servicios.PagoSrv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaV1.Controllers
{
    [ApiController]
    [Route("dashboard/Api/[Controller]")]
    public class RegistrarPagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;
        public RegistrarPagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }


        [Authorize]
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



        [Authorize]
        [HttpPost("Pagar-ventas-deuda")]
        public async Task<IActionResult> RegistraVentasConDeudaAPagar([FromBody] VentasAPagarConDeudaDto deudaVenta, CancellationToken ct) {
            try
            {
                await _pagoService.PagarDeudasVenta(deudaVenta, ct);
                return Ok(new { message = "pago registrado correctamente." });
            }
            catch(Exception ex) {
                return BadRequest(new { message = $"Error al registrar el pago: {ex.Message}" });
            }

        }

    }
}
