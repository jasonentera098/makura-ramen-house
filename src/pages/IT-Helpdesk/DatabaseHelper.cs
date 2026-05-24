using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Threading.Tasks;
using IT_Helpdesk.Helpers;

namespace IT_Helpdesk
{
    /// <summary>
    /// Provides the database connection string and path configuration.
    /// Delegates to <see cref="AppSettings"/> so the path can be overridden via appsettings.ini.
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// Full path to the MS Access database file (local or UNC).
        /// </summary>
        public static string DatabasePath => AppSettings.DatabasePath;

        /// <summary>
        /// OleDb connection string for the MS Access database.
        /// </summary>
        public static string ConnectionString => AppSettings.ConnectionString;

        /// <summary>
        /// Attempts to open and immediately close a connection to the configured database.
        /// Returns <c>(true, null)</c> on success, or <c>(false, userFriendlyMessage)</c> on failure.
        /// This method never throws — all exceptions are caught and converted to friendly messages.
        /// </summary>
        public static (bool Success, string? ErrorMessage) TestConnection()
        {
            try
            {
                // Quick pre-check: if the path looks like a local file and it doesn't exist,
                // give a specific message before OleDb even tries to open it.
                string dbPath = DatabasePath;
                if (!dbPath.StartsWith(@"\\") && !File.Exists(dbPath))
                {
                    return (false,
                        "Cannot connect to the helpdesk database. " +
                        "Please verify the database file is accessible on the network share.");
                }

                using var connection = new OleDbConnection(ConnectionString);
                connection.Open();
                // Connection succeeded — close immediately.
                return (true, null);
            }
            catch (Exception ex)
            {
                // Log the raw exception for diagnostics.
                System.Diagnostics.Debug.WriteLine(
                    $"[DatabaseConfig.TestConnection] {ex.GetType().FullName}: {ex.Message}");

                var friendly = DatabaseConnectionException.FromException(ex, DatabasePath);
                return (false, friendly.Message);
            }
        }
    }
}
