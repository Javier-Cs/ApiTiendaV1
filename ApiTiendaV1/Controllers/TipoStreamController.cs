using ApiTiendaV1.DTOs.Stream;
using ApiTiendaV1.Servicios.Streams;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaV1.Controllers
{

    [ApiController]
    [Route("Api/[controller]")]
    public class TipoStreamController : ControllerBase
    {
        private readonly StreamService _streamService;

        public TipoStreamController(StreamService streamService)
        {
            _streamService = streamService;
        }

        [HttpGet]
        public async Task<ActionResult<StreamResponseDto>> GetTipoStream( string url) {

            try
            {
                var respuesta = await _streamService.GetStream(url);

                if (respuesta is null)
                {
                    return BadRequest("error en la peticion");
                }
                return Ok(respuesta);

            }
            catch (Exception ex) {
                return BadRequest(ex.ToString());
            }
        }
    }
}
