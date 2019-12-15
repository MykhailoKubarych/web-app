using System;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using WebShop.Api.Config;

namespace WebShop.Api.DataAccess
{ 
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
    
    public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
    {
        private readonly IConnectionString _connectionString;
        private IDbConnection _connection;

        public SqlConnectionFactory(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetOpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new NpgsqlConnection(_connectionString.Value);
                _connection.Open();
            }

            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Dispose();
            }
        }
    }
}