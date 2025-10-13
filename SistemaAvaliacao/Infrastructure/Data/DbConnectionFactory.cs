using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SistemaAvaliacao.Infrastructure.Data
{
    /// <summary>
    /// Implementação padrão da factory de conexões usando SqlConnection.
    /// </summary>
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString = null)
        {
            _connectionString = connectionString ?? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
