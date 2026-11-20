using Microsoft.Extensions.Logging;
using Npgsql;
using Trazent.Api.DataAccess;
using Trazent.Core.Infraestructure.Repositories;
using Trazent.Infraestructure.Common.Entities;
using System.Data;

namespace Trazent.Core.Implementations.Repositories
{
    public class SitesRepository : ISitesRepository
    {
        private readonly ILogger<SitesRepository> _logger;
        private NpgsqlConnection _connection = default!;
        private NpgsqlTransaction? _transaction;

        public SitesRepository(ILogger<SitesRepository> logger) =>  _logger = logger;

        public void SetConnection(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        {
            _connection = connection ?? throw new ArgumentException(nameof(connection));
            _transaction = transaction;
        }

        #region CreateSiteAsync
        public async Task CreateSiteAsync(Sites site)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection not initializaed.");

            const string sql = "create_site";

            using var command = new NpgsqlCommand(sql, _connection, _transaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("p_company_id", site.Company.Id);
            command.Parameters.AddWithValue("p_name", site.Name);
            command.Parameters.AddWithValue("p_addreess", site.Address);
            command.Parameters.AddWithValue("p_latitude", site.Latitude);
            command.Parameters.AddWithValue("p_longitude", site.Longitude);
            command.Parameters.AddWithValue("p_radius_meters", site.RadiusMeter);

            await command.ExecuteNonQueryAsync();

        }
        #endregion
        #region GetSiteById
        public async Task<Sites> GetSiteByIdAsync(Guid id)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection not initialized.");

            const string sql = "get_site_by_id";
            Sites site = new Sites();

            using var command = new NpgsqlCommand(sql, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("p_id", id);

            await using var reader = await command.ExecuteReaderAsync();

            while(await reader.ReadAsync())
            {
                site.Id = reader.GetGuid(reader.GetOrdinal("id"));
                site.Name = reader.GetString(reader.GetOrdinal("name"));
                site.Company.Id = reader.GetGuid(reader.GetOrdinal("company_id"));
                site.Address = reader.GetString(reader.GetOrdinal("address"));
                site.IsActive = reader.GetBoolean(reader.GetOrdinal("is_active"));
                site.Longitude = reader.GetDouble(reader.GetOrdinal("longitude"));
                site.RadiusMeter = reader.GetInt16(reader.GetOrdinal("radius_meters"));
            };

            return site;
        }
        #endregion
        #region UpdateSiteAsync
        public async Task<Sites> UpdateSiteAsync(Sites data)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection not initialized.");

            const string sql = "update_site";

            Sites site = new Sites();

            using var command = new NpgsqlCommand(sql, _connection, _transaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("p_id", data.Id);
            command.Parameters.AddWithValue("p_name", data.Name);
            command.Parameters.AddWithValue("p_company_id", data.Company.Id);
            command.Parameters.AddWithValue("p_longitude", data.Longitude);
            command.Parameters.AddWithValue("p_latitude", data.Latitude);
            command.Parameters.AddWithValue("p_radius_meters", data.RadiusMeter);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                site.Id = reader.GetGuid(reader.GetOrdinal("id"));
                site.Name = reader.GetString(reader.GetOrdinal("name"));
                site.Company.Id = reader.GetGuid(reader.GetOrdinal("company_id"));
                site.Longitude = reader.GetDouble(reader.GetOrdinal("longitude"));
                site.Latitude = reader.GetDouble(reader.GetOrdinal("latitude"));
                site.RadiusMeter = reader.GetInt32(reader.GetOrdinal("radius_meters"));
                site.Address = reader.GetString(reader.GetOrdinal("address"));
                site.CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
                site.UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"));
            }

            return site;
        }

        #endregion

    }
}
