using ApiTiendaV1.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaV1.Controllers
{
    [ApiController]
    [Route("Api/[Controller]")]
    public class RegistrarPagoController : ControllerBase

    {
        [HttpPost]
        public async Task<IActionResult> RegistrarVentasPagadas(
            [FromBody] ReporteClienteVentaDto reporteClienteVentaDto
            , CancellationToken ct) {


            return null;
        }
    }
}
