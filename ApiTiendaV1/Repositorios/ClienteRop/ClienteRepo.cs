using ApiTiendaV1.Data;
using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.ClienteDt;
using ApiTiendaV1.Modelos;
using ApiTiendaV1.Servicios.VentaSrv;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiTiendaV1.Repositorios.ClienteRop
{
    public class ClienteRepo : IClienteRepo
    {
        private readonly ISqlConnectionFactory _sqlconnection;

        public ClienteRepo(ISqlConnectionFactory sqlconnection)
        {
            _sqlconnection = sqlconnection;
        }

        public async Task<bool> ActualizarCliAsync(int idCliente, ClienteUpDto dto, CancellationToken ct = default)
        {
            var sql =  "UPDATE clientes SET";
            var updates = new List<string>();

            if (dto.nombre != null) updates.Add("nombre = @nombre");
            if (dto.telefono != null) updates.Add("telefono = @telefono");
            if (dto.tipo != null) updates.Add("tipo = @tipo");
            if (dto.estado.HasValue) updates.Add("estado = @estado");

            sql += " " + string.Join(", ", updates);
            sql += " WHERE id_cliente = @idCliente";

            using var connection = _sqlconnection.CreateConnection();


            var rows = await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new { 
                        idCliente,
                        dto.nombre,
                        dto.telefono,
                        dto.tipo,
                        dto.estado
                    },
                    cancellationToken:ct
                    )
                );
            return rows > 0;
        }

        public async Task<int> CrearCliAsync(ClienteCrearDto dto, CancellationToken ct = default)
        {
            const string sql = @"
                INSERT INTO clientes (nombre, telefono, email, tipo, estado, fecha_creacion)
                VALUES (@nombre, @telefono, @email, @tipo, @estado, @fecha_creacion);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _sqlconnection.CreateConnection();

            var id = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, dto, cancellationToken: ct));
            return id;
        }

        public async Task<bool> EliminarCliAsync(int idCliente, CancellationToken ct = default)
        {
            const string sql = @"
                UPDATE clientes
                SET estado = 0
                WHERE id_cliente = @idCliente;
            ";

            using var connection = _sqlconnection.CreateConnection();
            var rows = await connection.ExecuteAsync(
                    new CommandDefinition(sql, new {idCliente }, cancellationToken: ct)
                );
            return rows > 0;
        }

        public async Task<ClienteCompleDto?> ObtenerCliPorIdAsync(int idCliente, CancellationToken ct = default)
        {
            const string sql = @"SELECT 
                id_cliente,
                nombre,
                telefono,
                email,
                tipo,
                estado,
                fecha_creacion
            FROM clientes
            WHERE id_cliente = @idCliente;";
            using var connection = _sqlconnection.CreateConnection();
            var cliente = await connection.QueryFirstOrDefaultAsync<ClienteCompleDto>(
                    new CommandDefinition(sql, new {idCliente}, cancellationToken: ct)
                );

            return cliente;            
        }

        public async Task<IEnumerable<ClienteDto>> ObtenerTodosLosCliAsync(CancellationToken ct = default)
        {
            const string sql = @"
                SELECT 
                id_cliente,
                nombre,
                tipo,
                estado,
                fecha_creacion
            FROM clientes
            ORDER BY nombre;
                ";

            using var connections = _sqlconnection.CreateConnection();
            var todosLosClientes =  await connections.QueryAsync<ClienteDto>(
                    new CommandDefinition(sql, cancellationToken: ct)
                );
            return todosLosClientes.AsList();
        }


        //---
        public async Task<IEnumerable<ClienteDto>> ObtenerTodosLosCLIEstadoAsync(bool estadoCliente, string tipocliente, CancellationToken ct = default)
        {
            const string sql = @"
                        SELECT 
                        id_cliente,
                        nombre,
                        tipo,
                        estado,
                        fecha_creacion
                    FROM clientes
                    WHERE estado = @estadoCliente AND tipo = @tipocliente;";
            using var connection = _sqlconnection.CreateConnection();
            var todoLosClientesEstado =  await connection.QueryAsync<ClienteDto>(
                new CommandDefinition(sql, new { estadoCliente, tipocliente}, cancellationToken: ct)
                );
            return todoLosClientesEstado.AsList();

        }

        public async Task<IEnumerable<ClienteSearchDto>> BuscarCliAsync(string nombre, CancellationToken ct = default) {

            const string sqlb = @"
                        select top 10 id_cliente, nombre 
                        from clientes where nombre 
                        like '%'+@nombre+'%' 
                        order by nombre;";

            using var connection = _sqlconnection.CreateConnection();
            var clientes  = await connection.QueryAsync<ClienteSearchDto>(
                new CommandDefinition(sqlb, new { nombre = $"%{nombre}%" }, cancellationToken: ct)
                );
            return clientes.AsList();   
        }

    }
}





