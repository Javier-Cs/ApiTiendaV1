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
            const string sql = @"
                INSERT INTO ventas (
                    id_cliente,
                    nombre_vendedor,
                    descripcion_venta,
                    tipo_venta,
                    estado_venta,
                    efectivo_recibido,
                    monto_total_Venta,
                    monto_vuelto,
                    fecha_venta
                )
                VALUES (
                    @id_cliente,
                    @nombre_vendedor,
                    @descripcion_venta,
                    @tipo_venta,
                    @estado_venta,
                    @efectivo_recibido,
                    @monto_total_Venta,
                    @monto_vuelto,
                    @fecha_venta
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = _sqlconnection.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, dto, cancellationToken: ct)
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
    }
}
