using System;
using System.Data.OleDb;
using System.IO;

namespace IT_Helpdesk.Helpers
{
    /// <summary>
    /// Represents a database connection or operation failure with a user-friendly,
    /// LAN-specific message. Wraps the original <see cref="OleDbException"/> or
    /// other low-level exception so callers can display actionable guidance without
    /// exposing raw .NET exception text.
    /// </summary>
    public class DatabaseConnectionException : Exception
    {
        /// <summary>
        /// Initialises a new instance with a user-friendly message and the original exception.
        /// </summary>
        public DatabaseConnectionException(string userFriendlyMessage, Exception? innerException = null)
            : base(userFriendlyMessage, innerException)
        {
        }

        // ── Factory helpers ──────────────────────────────────────────────────

        /// <summary>
        /// Inspects <paramref name="ex"/> and returns a <see cref="DatabaseConnectionException"/>
        /// whose message is appropriate for display to an end-user on a LAN-only system.
        /// </summary>
        /// <param name="ex">The raw exception thrown by OleDb or the file system.</param>
        /// <param name="databasePath">The configured database path (used in log/debug output only).</param>
        public static DatabaseConnectionException FromException(Exception ex, string? databasePath = null)
        {
            string friendlyMessage = BuildFriendlyMessage(ex, databasePath);
            return new DatabaseConnectionException(friendlyMessage, ex);
        }

        // ── Private helpers ──────────────────────────────────────────────────

        private static string BuildFriendlyMessage(Exception ex, string? databasePath)
        {
            // ── OleDb-specific error codes ───────────────────────────────────
            if (ex is OleDbException oleDbEx)
                return MapOleDbException(oleDbEx, databasePath);

            // ── File / path not found ────────────────────────────────────────
            if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                return "Cannot connect to the helpdesk database. " +
                       "Please verify the database file is accessible on the network share.";

            // ── Network / IO errors ──────────────────────────────────────────
            if (ex is IOException ioEx)
            {
                string msg = ioEx.Message ?? string.Empty;
                if (ContainsAny(msg, "network", "unc", "\\\\", "unreachable", "path"))
                    return "The network location for the helpdesk database is unavailable. " +
                           "Please check your LAN connection and try again.";

                return "A file system error occurred while accessing the helpdesk database. " +
                       "Please check your LAN connection and contact IT support if the problem persists.";
            }

            // ── Unauthorised access ──────────────────────────────────────────
            if (ex is UnauthorizedAccessException)
                return "Access to the helpdesk database was denied. " +
                       "Please contact your IT administrator to verify your permissions.";

            // ── Fallback ─────────────────────────────────────────────────────
            return "A database error occurred. " +
                   "Please check your LAN connection and contact IT support if the problem persists.";
        }

        private static string MapOleDbException(OleDbException ex, string? databasePath)
        {
            string msg = ex.Message ?? string.Empty;

            // ── Driver / ISAM not installed ──────────────────────────────────
            if (ContainsAny(msg, "installable isam", "isam", "provider", "ace.oledb", "jet.oledb"))
                return "The Microsoft Access database driver is not installed on this computer. " +
                       "Please contact your IT administrator to install the Microsoft Access Database Engine.";

            // ── File not found / cannot open ─────────────────────────────────
            if (ContainsAny(msg, "cannot open database", "could not find file",
                                 "file not found", "not a valid path"))
                return "Cannot connect to the helpdesk database. " +
                       "Please verify the database file is accessible on the network share.";

            // ── Network / UNC path unreachable ───────────────────────────────
            if (ContainsAny(msg, "network", "\\\\", "unc", "unreachable",
                                 "network path", "network name"))
                return "The network location for the helpdesk database is unavailable. " +
                       "Please check your LAN connection and try again.";

            // ── Database locked / exclusive use ──────────────────────────────
            if (ContainsAny(msg, "locked", "exclusive", "already in use",
                                 "sharing violation", "ldb", "laccdb"))
                return "The helpdesk database is currently in use by another process. " +
                       "Please try again in a moment.";

            // ── Password / encryption ────────────────────────────────────────
            if (ContainsAny(msg, "password", "encrypted", "not a valid password"))
                return "The helpdesk database requires a password that is not configured. " +
                       "Please contact your IT administrator.";

            // ── Disk full / write error ──────────────────────────────────────
            if (ContainsAny(msg, "disk", "space", "write", "read-only"))
                return "The helpdesk database cannot be written to. " +
                       "The network share may be read-only or the disk may be full. " +
                       "Please contact your IT administrator.";

            // ── Generic OleDb fallback ───────────────────────────────────────
            return "A database error occurred. " +
                   "Please check your LAN connection and contact IT support if the problem persists.";
        }

        /// <summary>Returns <c>true</c> if <paramref name="source"/> contains any of the given keywords (case-insensitive).</summary>
        private static bool ContainsAny(string source, params string[] keywords)
        {
            foreach (string kw in keywords)
            {
                if (source.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }
    }
}
