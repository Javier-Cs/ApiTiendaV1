using System.Data;

namespace ApiTiendaV1.Data
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
