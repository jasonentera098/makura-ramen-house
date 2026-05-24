using System;
using System.IO;

namespace IT_Helpdesk
{
    /// <summary>
    /// Reads and exposes application configuration from <c>appsettings.ini</c>.
    /// Supports both a local relative path and a LAN UNC path for the MS Access database.
    ///
    /// INI file format (appsettings.ini, placed next to the executable):
    /// <code>
    /// [Database]
    /// ; Use a UNC path for LAN access, e.g.: \\Server\Share\HelpdeskDB.accdb
    /// ; Leave blank (or omit) to fall back to the local default path.
    /// DatabasePath=
    /// </code>
    /// </summary>
    public static class AppSettings
    {
        private const string IniFileName = "appsettings.ini";
        private const string SectionDatabase = "Database";
        private const string KeyDatabasePath = "DatabasePath";

        // Default local path: <AppDir>\Data\HelpdeskDB.accdb
        private static readonly string DefaultDatabasePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HelpdeskDB.accdb");

        private static string? _resolvedDatabasePath;

        /// <summary>
        /// Resolved path to the MS Access database file.
        /// Priority: INI file value → default local path.
        /// </summary>
        public static string DatabasePath
        {
            get
            {
                if (_resolvedDatabasePath == null)
                    _resolvedDatabasePath = ResolveDatabasePath();
                return _resolvedDatabasePath;
            }
        }

        /// <summary>
        /// OleDb connection string built from <see cref="DatabasePath"/>.
        /// </summary>
        public static string ConnectionString =>
            $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={DatabasePath};Persist Security Info=False;";

        /// <summary>
        /// Forces a re-read of the INI file on the next access to <see cref="DatabasePath"/>.
        /// Useful when the INI file is updated at runtime.
        /// </summary>
        public static void Reload() => _resolvedDatabasePath = null;

        // ── Private helpers ──────────────────────────────────────────────────

        private static string ResolveDatabasePath()
        {
            string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, IniFileName);

            if (File.Exists(iniPath))
            {
                string? value = ReadIniValue(iniPath, SectionDatabase, KeyDatabasePath);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Expand environment variables so paths like %USERPROFILE% work too
                    string expanded = Environment.ExpandEnvironmentVariables(value.Trim());
                    return expanded;
                }
            }

            return DefaultDatabasePath;
        }

        /// <summary>
        /// Minimal INI parser — reads the value for <paramref name="key"/> under
        /// <paramref name="section"/>. Returns <c>null</c> when not found.
        /// </summary>
        private static string? ReadIniValue(string iniPath, string section, string key)
        {
            bool inSection = false;

            foreach (string rawLine in File.ReadLines(iniPath))
            {
                string line = rawLine.Trim();

                // Skip blank lines and comments (; or #)
                if (line.Length == 0 || line[0] == ';' || line[0] == '#')
                    continue;

                // Section header: [SectionName]
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    string sectionName = line.Substring(1, line.Length - 2).Trim();
                    inSection = string.Equals(sectionName, section, StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (!inSection)
                    continue;

                // Key=Value
                int eqIndex = line.IndexOf('=');
                if (eqIndex < 1)
                    continue;

                string lineKey = line.Substring(0, eqIndex).Trim();
                if (string.Equals(lineKey, key, StringComparison.OrdinalIgnoreCase))
                {
                    // Value may be empty (key present but blank → use default)
                    string lineValue = line.Substring(eqIndex + 1).Trim();

                    // Strip inline comment after the value
                    int commentIdx = lineValue.IndexOf(';');
                    if (commentIdx >= 0)
                        lineValue = lineValue.Substring(0, commentIdx).Trim();

                    return lineValue;
                }
            }

            return null;
        }
    }
}
