using System.Data;

namespace SistemaAvaliacao.Infrastructure.Data
{
    /// <summary>
    /// Interface para factory de conexões de banco de dados.
    /// </summary>
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
