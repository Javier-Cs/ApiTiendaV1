using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.ClienteDt;
using ApiTiendaV1.Repositorios.ClienteRop;

namespace ApiTiendaV1.Servicios.ClienteSrv
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepo _clienteRepo;

        public ClienteService(IClienteRepo clienteRepo)
        {
            _clienteRepo = clienteRepo;
        }

        public async Task<bool> Actualizar_CliAsync(int idCliente, ClienteUpDto dto, CancellationToken ct = default)
        {
            var cliente = await _clienteRepo.ObtenerCliPorIdAsync(idCliente, ct);
            if (cliente == null) {
                throw new KeyNotFoundException("Cliente no encontrado");
            }
            if (dto.nombre == null && dto.telefono == null && dto.tipo == null && !dto.estado.HasValue)
                throw new ArgumentException("No hay datos para actualizar");
            return await _clienteRepo.ActualizarCliAsync(idCliente, dto, ct);
        }

        public async Task<int> Crear_CliAsync(ClienteCrearDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.nombre))
            {
                throw new ArgumentException("El nombre del cliente es obligatorio");
            }
            if (string.IsNullOrWhiteSpace(dto.email) || !dto.email.Contains("@"))
                throw new ArgumentException("Email inválido");

            if (string.IsNullOrWhiteSpace(dto.tipo))
                throw new ArgumentException("Tipo de cliente inválido");

            dto.estado = true;
            dto.fecha_creacion = DateTime.UtcNow;

            return await _clienteRepo.CrearCliAsync(dto, ct);
        }

        public async Task<bool> Eliminar_CliAsync(int idCliente, CancellationToken ct = default)
        {
            var cliente = await _clienteRepo.ObtenerCliPorIdAsync(idCliente, ct);
            if (cliente == null) {
                throw new KeyNotFoundException("Cliente no encontrado");
            }
            if (!cliente.estado)
                throw new InvalidOperationException("El cliente ya está inactivo");

            return await _clienteRepo.EliminarCliAsync(idCliente, ct);
        }

        public Task<ClienteCompleDto?> Obtener_CliPorIdAsync(int idCliente, CancellationToken ct = default)
        {
           return  _clienteRepo.ObtenerCliPorIdAsync(idCliente, ct);
        }

        public Task<IEnumerable<ClienteDto>> ObtenerTodos_LosCliAsync(CancellationToken ct = default)
        {
            return _clienteRepo.ObtenerTodosLosCliAsync(ct);
        }

        public Task<IEnumerable<ClienteDto>> ObtenerTodos_LosCLIEstadoAsync(bool estadoCliente, string tipocliente, CancellationToken ct = default)
        {
            return _clienteRepo.ObtenerTodosLosCLIEstadoAsync(estadoCliente, tipocliente, ct);
        }

        public Task<IEnumerable<ClienteSearchDto>> Buscar_CliAsync(string tipocliente, CancellationToken ct = default)
        {
            return _clienteRepo.BuscarCliAsync(tipocliente, ct);
        }
    }
}
