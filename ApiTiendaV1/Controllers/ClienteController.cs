using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.ClienteDt;
using ApiTiendaV1.Servicios.ClienteSrv;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaV1.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // POST: api/clientes

        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] ClienteCrearDto dto,
            CancellationToken ct)
        {
            var id = await _clienteService.Crear_CliAsync(dto, ct);
            return CreatedAtAction(nameof(ObtenerPorId), new { idCliente = id }, new { id });
        }

        // GET: api/clientes
        [HttpGet("All")]
        public async Task<IActionResult> ObtenerTodos(CancellationToken ct)
        {
            var clientes = await _clienteService.ObtenerTodos_LosCliAsync(ct);
            return Ok(clientes);
        }

        // GET: api/clientes/{idCliente}
        [HttpGet("{idCliente:int}")]
        public async Task<IActionResult> ObtenerPorId(
            int idCliente,
            CancellationToken ct)
        {
            var cliente = await _clienteService.Obtener_CliPorIdAsync(idCliente, ct);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        // PUT: api/clientes/{idCliente}
        [HttpPut("{idCliente:int}")]
        public async Task<IActionResult> Actualizar(
            int idCliente,
            [FromBody] ClienteUpDto dto,
            CancellationToken ct)
        {
            await _clienteService.Actualizar_CliAsync(idCliente, dto, ct);
            return NoContent();
        }

        // DELETE: api/clientes/{idCliente}
        [HttpDelete("{idCliente:int}")]
        public async Task<IActionResult> Eliminar(
            int idCliente,
            CancellationToken ct)
        {
            await _clienteService.Eliminar_CliAsync(idCliente, ct);
            return NoContent();
        }

        [HttpGet("estado/{estadoCliente:bool}/deuda/{tipocliente}")]
        public async Task<IActionResult> ObtenerTodosLosEstados(
            bool estadoCliente,
            string tipocliente,
            CancellationToken ct)
        {
            var cliente = await _clienteService.ObtenerTodos_LosCLIEstadoAsync(estadoCliente,tipocliente, ct);
            return Ok(cliente);
        }

        [HttpOptions]
        public IActionResult Options()
        {
            return Ok();
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarCliente([FromQuery]string nombre,CancellationToken ct){
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 2)
                return Ok(Enumerable.Empty<ClienteSearchDto>);
            var clientes = await _clienteService.Buscar_CliAsync(nombre, ct);
            return Ok(clientes);
        }
    }
}
