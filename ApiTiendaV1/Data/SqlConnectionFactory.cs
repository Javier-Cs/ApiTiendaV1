using Microsoft.Extensions.Primitives;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiTiendaV1.Data
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly String _connectionString;

        public SqlConnectionFactory(IConfiguration config) { 
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
        
    }
}
