using ApiTiendaV1.Data;
using ApiTiendaV1.DTOs.Auths;
using Dapper;
using System.Data;

namespace ApiTiendaV1.Repositorios.Auth
{
    public class UserAuthRepo
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public UserAuthRepo(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }


        public async Task<Usuario?> ObtenerUsuarioEmail(string email, IDbTransaction tx = null, CancellationToken ct = default) {
            const string sql = @"
                SELECT 
                    id_usuario,
                    nombre as nombre,
                    email_user as email,
                    passHass as passhass,
                    rol_usuario as rol,
                    estado as estado,
                    telefono as telefono,
                    is_deleted as is_deleted
                FROM usuarios
                WHERE email_user = @email
                    AND is_deleted = 0
                    AND estado = 1
            ";

            using var connection = _sqlConnectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                new CommandDefinition(
                    sql,
                    new {email},
                    transaction: tx,
                    cancellationToken: ct
                )
            );
        }


        public async Task<Usuario?> CrearUsuario(Usuario user, IDbTransaction tr = null, CancellationToken ct = default) {
            const string sql = @"
                INSERT INTO usuarios(
                    nombre_user,
                    email_user,
                    passHass,
                    telefono,
                    fecha_creacion
                )VALUES(
                    @nombre,
                    @email,
                    @passhass,
                    @telefono,
                    fecha_creacion
                );
            ";

            using var connection = _sqlConnectionFactory.CreateConnection();

            var respuesta = await connection.QueryFirstOrDefaultAsync<Usuario>(
                new CommandDefinition(
                    sql,
                    new {
                        nombre_user = user.nombre,
                        email_user = user.email,
                        passHass = user.passhass,
                        telefono = user.telefono,
                        fecha_creacion = user.fecha_creacion
                    },
                    transaction: tr,
                    cancellationToken:ct
                )
            );

            return respuesta;
        }
    }
}
