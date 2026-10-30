using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Trazent.DataAccess;


namespace Trazent.Api.DataAccess
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseConnection> _logger;

        public DatabaseConnection(IConfiguration configuration, ILogger<DatabaseConnection> logger)
        {
            _connectionString = configuration.GetConnectionString("ApplicationDBServer") 
                ?? throw new ArgumentNullException("Connection string 'ApplicationDBServer' not found in configuration.");
            _logger = logger;
        }

        public async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            try
            {
                var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                _logger.LogInformation("Database connection established successfully.");
                return connection;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection failed. NpgsqlException: {Message}", ex.Message);
                throw new DatabaseConnectionException("Failed to connect to the database.", ex);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Database connection timed out.");
                throw new DatabaseConnectionException("Database connection timed out.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when connecting to the database.");
                throw new DatabaseConnectionException("Unexpected database connection error.", ex);
            }
        }
    }
}
