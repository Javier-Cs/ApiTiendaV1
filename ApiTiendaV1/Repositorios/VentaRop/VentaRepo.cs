using ApiTiendaV1.Data;
using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;
using Dapper;

namespace ApiTiendaV1.Repositorios.VentaRop
{
    public class VentaRepo : IVentaRepo
    {
        private readonly ISqlConnectionFactory _sqlconnection;

        public VentaRepo(ISqlConnectionFactory sqlconnection)
        {
            _sqlconnection = sqlconnection;
        }

        public async Task<int> CrearVenAsync(VentaCrearDto dto, CancellationToken ct = default)
        {
            string estado_venta = "";
            if (dto.tipo_venta == TipoVenta.Contado) {
                estado_venta = EstadoVenta.Pagado;
            } else if (dto.tipo_venta == TipoVenta.Credito) {
                estado_venta = EstadoVenta.Deuda;
            }
            const string sql = @"
                INSERT INTO ventas (
                    id_cliente,
                    nombre_vendedor,
                    descripcion_venta,
                    tipo_venta,
                    estado_venta,
                    efectivo_recibido,
                    monto_total_Venta,
                    monto_vuelto
                )
                VALUES (
                    @id_cliente,
                    @nombre_vendedor,
                    @descripcion_venta,
                    @tipo_venta,
                    @estado_venta,
                    @efectivo_recibido,
                    @monto_total_Venta,
                    @monto_vuelto
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = _sqlconnection.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, 
                new {
                    dto.id_cliente,
                    dto.nombre_vendedor,
                    dto.descripcion_venta,
                    dto.tipo_venta,
                    estado_venta,               // 👈 AQUÍ está la clave
                    dto.efectivo_recibido,
                    dto.monto_total_Venta,
                    dto.monto_vuelto
                }, cancellationToken: ct)
            );
        }

        public async Task<bool> EliminarVentAsync(int idVenta, int idCliente, CancellationToken ct = default)
        {
            const string sql = @"
                UPDATE ventas
                SET estado_venta = 'PAGADO'
                WHERE id_venta = @idVenta
                  AND id_cliente = @idCliente;
            ";

            using var connection = _sqlconnection.CreateConnection();

            var rows = await connection.ExecuteAsync(
                    new CommandDefinition(sql, new {idVenta, idCliente}, cancellationToken: ct)
                );
            return rows > 0;
        }


        public async Task<IEnumerable<VentaDto>> ObtenerTodasLasVenAsync(CancellationToken ct = default)
        {
            const int meses = 2;
            const string sql = @"SELECT 
                id_venta,
                id_cliente,
                nombre_vendedor,
                tipo_venta,
                estado_venta,
                monto_total_Venta,
                fecha_venta    
                FROM ventas
                WHERE fecha_venta >= DATEADD(MONTH, -@meses, GETDATE())
                ORDER BY fecha_venta DESC";
            using var connections = _sqlconnection.CreateConnection();
            var ventas = await connections.QueryAsync<VentaDto>(
                    new CommandDefinition(sql,new { meses }, cancellationToken: ct)
                );
            return ventas.AsList();
        }

        // obtener toda la informacion de la venta de una venta 
        public async Task<VentaCompletaDto?> ObtenerVenPorIdVenAsync(int idVenta, CancellationToken ct = default)
        {
            const string sql = @"SELECT
            id_cliente,
            id_venta,
            nombre_vendedor,
            descripcion_venta,
            tipo_venta,
            estado_venta,
            efectivo_recibido,
            monto_total_Venta,
            monto_vuelto,
            fecha_venta
        FROM ventas
        WHERE id_venta = @idVenta;";

            using var connection = _sqlconnection.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<VentaCompletaDto>(
                new CommandDefinition(sql, new { idVenta }, cancellationToken: ct)
                );
        }


        //-----

        // este metodo es solo para mostrar las ventas que tienen deuda por credito
        public async Task<IEnumerable<VentaDto>> ObtenerTodasVenConDeudaAsync(CancellationToken ct = default)
        {
            const string sql = @"SELECT
            id_venta,
            id_cliente,
            nombre_vendedor,
            tipo_venta,
            estado_venta,
            monto_total_Venta,
            fecha_venta
        FROM ventas
        WHERE estado_venta = @estado AND tipo_venta = @tipo
        ORDER BY fecha_venta DESC;";
            using var connection = _sqlconnection.CreateConnection();
            var todasLasventasconDeuda = await connection.QueryAsync<VentaDto>(
                    new CommandDefinition(sql,new {estado = EstadoVenta.Deuda, tipo= TipoVenta.Credito }, cancellationToken:ct)
                );
            return todasLasventasconDeuda.AsList();
        }



        public async Task<IEnumerable<VentaDto>> ObtenerVenDeudaPorClienteAsync(int idCliente, string estadoVenta, string tipoVenta, CancellationToken ct = default)
        {
            const string sql = @"SELECT
            id_venta,
            id_cliente,
            nombre_vendedor,
            tipo_venta,
            estado_venta,
            monto_total_Venta,
            fecha_venta
        FROM ventas
        WHERE id_cliente = @idCliente AND estado_venta = @estadoVenta  AND tipo_venta = @tipoVenta
        ORDER BY fecha_venta DESC;";

            using var connection = _sqlconnection.CreateConnection();

            var deudasPorCliente = await connection.QueryAsync<VentaDto>(
                    new CommandDefinition(sql, new { idCliente , estadoVenta, tipoVenta}, cancellationToken: ct)
                );
            return deudasPorCliente.AsList();
        }

        public async Task<IEnumerable<VentaDto>> ObtenerVentasPorClienteAsync(int idCliente, CancellationToken ct = default)
        {
            const string sql = @"SELECT
            id_venta,
            id_cliente,
            nombre_vendedor,
            tipo_venta,
            estado_venta,
            monto_total_Venta,
            fecha_venta
        FROM ventas
        WHERE id_cliente = @idCliente 
        ORDER BY fecha_venta DESC;";

            using var connection = _sqlconnection.CreateConnection();

            var ventaPorCliente = await connection.QueryAsync<VentaDto>(
                new CommandDefinition(sql, new {idCliente },cancellationToken : ct));

            return ventaPorCliente.AsList();
        }

        public async Task<bool> ActualizarVentaAsync(int idVenta, VentaUpDto dto, CancellationToken ct = default)
        {
            var updates = new List<string>();

            if (dto.descripcion_venta != null)
                updates.Add("descripcion_venta = @descripcion_venta");

            if (dto.tipo_venta != null)
                updates.Add("tipo_venta = @tipo_venta");

            if (dto.efectivo_recibido != null)
                updates.Add("efectivo_recibido = @efectivo_recibido");

            if (dto.monto_total_Venta != null)
                updates.Add("monto_total_Venta = @monto_total_Venta");

            decimal? vuelto = null;
            if (dto.efectivo_recibido.HasValue && dto.monto_total_Venta.HasValue) {
                vuelto = dto.efectivo_recibido.Value - dto.monto_total_Venta.Value;
                updates.Add("monto_vuelto = @vuelto");
            }

            var sql = $@"
                UPDATE ventas
                SET {string.Join(", ", updates)}
                WHERE id_venta = @idVenta";

            using var connection = _sqlconnection.CreateConnection();

            var rows = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new
                    {
                        idVenta,
                        dto.descripcion_venta,
                        dto.tipo_venta,
                        dto.efectivo_recibido,
                        dto.monto_total_Venta,
                        vuelto
                    },
                    cancellationToken: ct
                )
            );

            return rows > 0;
        }
    }
}
