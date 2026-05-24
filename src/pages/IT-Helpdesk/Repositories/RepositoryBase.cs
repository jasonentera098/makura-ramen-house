using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using IT_Helpdesk.Helpers;

namespace IT_Helpdesk
{
    /// <summary>
    /// Abstract base class for all repositories.
    /// Provides parameterized async query helpers using OleDb connections.
    /// Each method opens a fresh connection, executes the query, and disposes the connection.
    /// All OleDb and IO exceptions are wrapped in <see cref="DatabaseConnectionException"/>
    /// so callers receive user-friendly, LAN-specific error messages.
    /// </summary>
    public abstract class RepositoryBase
    {
        protected readonly string _connectionString;

        protected RepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Executes a scalar query and returns the first column of the first row,
        /// cast to <typeparamref name="T"/>. Returns default(T) when the result is null or DBNull.
        /// </summary>
        protected async Task<T?> ExecuteScalarAsync<T>(string query, OleDbParameter[]? parameters = null)
        {
            using var connection = new OleDbConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception ex) when (ex is OleDbException || ex is System.IO.IOException
                                        || ex is UnauthorizedAccessException
                                        || ex is System.IO.FileNotFoundException)
            {
                LogDatabaseError("ExecuteScalarAsync (open)", ex);
                throw DatabaseConnectionException.FromException(ex, DatabaseConfig.DatabasePath);
            }

            using var command = new OleDbCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters);

            try
            {
                var result = await command.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                    return default;

                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (OleDbException ex)
            {
                LogDatabaseError("ExecuteScalarAsync (execute)", ex);
                throw DatabaseConnectionException.FromException(ex, DatabaseConfig.DatabasePath);
            }
        }

        /// <summary>
        /// Executes a non-query command (INSERT, UPDATE, DELETE) and returns the number of rows affected.
        /// </summary>
        protected async Task<int> ExecuteNonQueryAsync(string query, OleDbParameter[]? parameters = null)
        {
            using var connection = new OleDbConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception ex) when (ex is OleDbException || ex is System.IO.IOException
                                        || ex is UnauthorizedAccessException
                                        || ex is System.IO.FileNotFoundException)
            {
                LogDatabaseError("ExecuteNonQueryAsync (open)", ex);
                throw DatabaseConnectionException.FromException(ex, DatabaseConfig.DatabasePath);
            }

            using var command = new OleDbCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters);

            try
            {
                return await command.ExecuteNonQueryAsync();
            }
            catch (OleDbException ex)
            {
                LogDatabaseError("ExecuteNonQueryAsync (execute)", ex);
                throw DatabaseConnectionException.FromException(ex, DatabaseConfig.DatabasePath);
            }
        }

        /// <summary>
        /// Executes a SELECT query and maps each row to <typeparamref name="T"/> using the provided mapper.
        /// </summary>
        protected async Task<List<T>> ExecuteReaderAsync<T>(
            string query,
            Func<OleDbDataReader, T> mapper,
            OleDbParameter[]? parameters = null)
        {
            var results = new List<T>();

            using var connection = new OleDbConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
            }
            catch (Exception ex) when (ex is OleDbException || ex is System.IO.IOException
                                        || ex is UnauthorizedAccessException
                                        || ex is System.IO.FileNotFoundException)
            {
                LogDatabaseError("ExecuteReaderAsync (open)", ex);
                throw DatabaseConnectionException.FromException(ex, DatabaseConfig.DatabasePath);
            }

            using var command = new OleDbCommand(query, connection);
            if (parameters != null)
                command.Parameters.AddRange(parameters);

            try
            {
                using var reader = (OleDbDataReader)await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                    results.Add(mapper(reader));
            }
            catch (OleDbException ex)
            {
                LogDatabaseError("ExecuteReaderAsync (read)", ex);
                throw DatabaseConnectionException.FromException(ex, DatabaseConfig.DatabasePath);
            }

            return results;
        }

        // ── Private helpers ──────────────────────────────────────────────────

        private static void LogDatabaseError(string context, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[RepositoryBase.{context}] {ex.GetType().FullName}: {ex.Message}");
        }
    }
}
