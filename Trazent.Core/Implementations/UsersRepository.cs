using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;
using Trazent.Core.Infraestructure.Repositories;
using Trazent.Infraestructure.Common.Entities;
using Trazent.Infraestructure.Common.Enums;

namespace Trazent.Core.Implementations
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ILogger<UsersRepository> _logger;
        NpgsqlConnection _connection = default!;
        NpgsqlTransaction _transaction;
        public UsersRepository(ILogger<UsersRepository> logger) => _logger = logger;

        public void SetConnection(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        {
            _connection = connection ?? throw new ArgumentException(nameof(connection));
            _transaction = transaction;
        }

        #region CreateUserAsync
        public async Task CreateUserAsync(Users user)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection not initialized.");

            const string sql = "create_user";

            await using var command = new NpgsqlCommand(sql, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("p_company_id", (object?)user.Company?.Id ?? DBNull.Value);
            command.Parameters.AddWithValue("p_full_name", user.FullName);
            command.Parameters.AddWithValue("p_role", (object?)(byte?)user.Role ?? DBNull.Value);
            command.Parameters.AddWithValue("p_email", user.Email);
            command.Parameters.AddWithValue("p_user_name", user.Username);
            command.Parameters.AddWithValue("p_password_hash", user.PasswordHash);
            command.Parameters.AddWithValue("p_security_stamp", user.SecurityStamp);
            command.Parameters.AddWithValue("p_concurrency_stamp", user.ConcurrencyStamp);
            command.Parameters.AddWithValue("p_phone_number", (object?)user.PhoneNumber ?? DBNull.Value);

            //var id = (Guid)(await command.ExecuteScalarAsync())!;
            await command.ExecuteNonQueryAsync();
         
        }
        #endregion
        #region GetUserByIdAsync
        public async Task<Users> GetUserByIdAsync(Guid id, Guid companyId)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection not initialized.");

            const string sql = "get_user_by_id";

            Users user = new Users();

            using var command = new NpgsqlCommand(sql, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("p_id", id);
            command.Parameters.AddWithValue("p_company_id", companyId);

            using var reader = await command.ExecuteReaderAsync();

            while(await reader.ReadAsync())
            {
                user.Id = reader.GetGuid(reader.GetOrdinal("id"));
                user.Company.Id = reader.GetGuid(reader.GetOrdinal("company_is"));
                user.Company.IsActive = reader.GetBoolean(reader.GetOrdinal("company_is_active"));
                user.FullName = reader.GetString(reader.GetOrdinal("full_name"));
                user.Role = (RoleTypeEnum)reader.GetInt16(reader.GetOrdinal("user_role"));
                user.Status = (StatusTypeEnum)reader.GetInt16(reader.GetOrdinal("user_status"));
                user.Username = reader.GetString(reader.GetOrdinal("user_name"));
                user.Email = reader.GetString(reader.GetOrdinal("email"));
                user.PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? null : reader.GetString(reader.GetOrdinal("phone_number"));
                user.SecurityStamp = reader.GetDateTime(reader.GetOrdinal("security_stamp"));
                user.LockoutEnd = reader.GetDateTime(reader.GetOrdinal("lockout_end"));
                user.CreatedAd = reader.GetDateTime(reader.GetOrdinal("created_at"));
                user.UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"));
            }
            return user;
        }
        #endregion
        #region GetUsersByCompany
        public async Task<List<Users>> GetUsersByCompanyAsync(Users data)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection not initialized");

            const string sql = "get_users_by_companies";

            var users = new List<Users>();
            int totalItems = 0;

            using var command = new NpgsqlCommand(sql, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("p_company_id", data.Company.Id);
            command.Parameters.AddWithValue("p_role", (byte)data.Role);
            command.Parameters.AddWithValue("p_status", (byte)data.Status);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new Users
                {
                    Id = reader.GetGuid(reader.GetOrdinal("user_id")),
                    FullName = reader.GetString(reader.GetOrdinal("full_name")),
                    Role = (RoleTypeEnum)reader.GetInt16(reader.GetOrdinal("user_role")),
                    Status = (StatusTypeEnum)reader.GetInt16(reader.GetOrdinal("user_status")),
                    Username = reader.GetString(reader.GetOrdinal("user_name")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                    LockoutEnd = reader.GetDateTime(reader.GetOrdinal("lockout_end")),
                    CreatedAd = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                });
            }
            return users;
        }
        #endregion
        #region UpdateUserAsync
        public async Task UpdateUserAsync(Users user)
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection not initialized.");

            const string sql = "update_user";

            using var command = new NpgsqlCommand(sql, _connection, _transaction)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("p_id", user.Id);
            command.Parameters.AddWithValue("p_company_id", user.Company.Id);
            command.Parameters.AddWithValue("p_full_name", user.FullName);
            command.Parameters.AddWithValue("p_phone_number", user.PhoneNumber);

            await command.ExecuteNonQueryAsync();
        }
        #endregion
        #region ChangeStatusById
        public async Task ChangeUserStatusByIdAsync(Users user )
        {
            if (_connection == null)
                throw new InvalidOperationException("Connection not initialized.");

            const string sql = "update_user";

            using var command = new NpgsqlCommand(sql, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("p_id", user.Id);
            command.Parameters.AddWithValue("p_company_id", user.Company.Id);
            command.Parameters.AddWithValue("p_status", (byte)user.Status);

            await command.ExecuteNonQueryAsync();
        }
        #endregion
    }
}


