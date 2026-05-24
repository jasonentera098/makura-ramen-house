using System;
using System.Data.OleDb;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IT_Helpdesk
{
    /// <summary>
    /// Handles creation of the MS Access database file and all required tables.
    /// Called at application startup to ensure the database is ready before any operations.
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Initializes the database: creates the .accdb file if missing, then creates all tables.
        /// </summary>
        /// <param name="dbPath">Absolute path to the .accdb file.</param>
        public static void Initialize(string dbPath)
        {
            // Ensure the Data/ directory exists
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            // Create the .accdb file if it doesn't exist
            if (!File.Exists(dbPath))
                CreateDatabaseFile(dbPath);

            // Create tables (skips any that already exist)
            CreateTables(dbPath);
        }

        private static void CreateDatabaseFile(string dbPath)
        {
            // Use ADOX Catalog via COM late-binding to create an empty .accdb file.
            // Requires Microsoft Access Database Engine (ACE) to be installed on the machine.
            var catalogType = Type.GetTypeFromProgID("ADOX.Catalog");
            if (catalogType == null)
                throw new InvalidOperationException(
                    "ADOX is not available. Please install the Microsoft Access Database Engine " +
                    "(https://www.microsoft.com/en-us/download/details.aspx?id=54920).");

            dynamic catalog = Activator.CreateInstance(catalogType)!;
            try
            {
                string connStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";
                catalog.Create(connStr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(catalog);
            }
        }

        private static void CreateTables(string dbPath)
        {
            string connStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";

            using var connection = new OleDbConnection(connStr);
            connection.Open();

            // Order matters: Departments before Users (Users references Departments),
            // Users before Tickets/TicketUpdates/Attachments/LoginLogs.
            var tableScripts = new[]
            {
                // 1. Departments
                @"CREATE TABLE Departments (
                    DepartmentID AUTOINCREMENT PRIMARY KEY,
                    DepartmentName TEXT(100) NOT NULL,
                    Description TEXT(255)
                )",

                // 2. Users
                @"CREATE TABLE Users (
                    UserID AUTOINCREMENT PRIMARY KEY,
                    FullName TEXT(100) NOT NULL,
                    Username TEXT(50) NOT NULL,
                    PasswordHash TEXT(255) NOT NULL,
                    Role TEXT(20) NOT NULL,
                    DepartmentID INTEGER NOT NULL,
                    ContactNumber TEXT(20),
                    CreatedAt DATETIME NOT NULL
                )",

                // 3. Tickets
                @"CREATE TABLE Tickets (
                    TicketID AUTOINCREMENT PRIMARY KEY,
                    Title TEXT(200) NOT NULL,
                    Description MEMO NOT NULL,
                    Category TEXT(50) NOT NULL,
                    Priority TEXT(20) NOT NULL,
                    Status TEXT(20) NOT NULL,
                    DateSubmitted DATETIME NOT NULL,
                    DateResolved DATETIME,
                    SubmittedBy INTEGER NOT NULL,
                    AssignedTo INTEGER,
                    DepartmentID INTEGER NOT NULL
                )",

                // 4. TicketUpdates
                @"CREATE TABLE TicketUpdates (
                    UpdateID AUTOINCREMENT PRIMARY KEY,
                    TicketID INTEGER NOT NULL,
                    UpdatedBy INTEGER NOT NULL,
                    Remarks MEMO NOT NULL,
                    UpdateDate DATETIME NOT NULL,
                    StatusChange TEXT(50)
                )",

                // 5. Attachments
                @"CREATE TABLE Attachments (
                    AttachmentID AUTOINCREMENT PRIMARY KEY,
                    TicketID INTEGER NOT NULL,
                    FilePath MEMO NOT NULL,
                    FileName TEXT(255) NOT NULL,
                    UploadedBy INTEGER NOT NULL,
                    UploadedAt DATETIME NOT NULL
                )",

                // 6. LoginLogs
                @"CREATE TABLE LoginLogs (
                    LogID AUTOINCREMENT PRIMARY KEY,
                    UserID INTEGER NOT NULL,
                    LoginTime DATETIME NOT NULL,
                    LogoutTime DATETIME
                )"
            };

            foreach (var script in tableScripts)
            {
                try
                {
                    using var cmd = new OleDbCommand(script, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (OleDbException ex) when (ex.Message.Contains("already exists"))
                {
                    // Table already exists — safe to skip
                }
            }

            SeedDepartments(connection);
            SeedAdminUser(connection);
            MigrateAccountStatus(connection);
            MigrateAttachmentFilePath(connection);
        }

        /// <summary>
        /// Adds AccountStatus column to Users table if it doesn't exist (migration for existing databases).
        /// </summary>
        private static void MigrateAccountStatus(OleDbConnection connection)
        {
            try
            {
                // Check if AccountStatus column exists by trying to select it
                using var checkCmd = new OleDbCommand("SELECT TOP 1 AccountStatus FROM Users", connection);
                checkCmd.ExecuteScalar();
                // Column exists, no migration needed
            }
            catch (OleDbException)
            {
                // Column doesn't exist, add it with default value 'Active'
                try
                {
                    using var alterCmd = new OleDbCommand(
                        "ALTER TABLE Users ADD COLUMN AccountStatus TEXT(20) DEFAULT 'Active'",
                        connection);
                    alterCmd.ExecuteNonQuery();

                    // Set all existing users to Active
                    using var updateCmd = new OleDbCommand(
                        "UPDATE Users SET AccountStatus = 'Active' WHERE AccountStatus IS NULL",
                        connection);
                    updateCmd.ExecuteNonQuery();
                }
                catch (OleDbException ex)
                {
                    // Log but don't fail - column might have been added by another process
                    System.Diagnostics.Debug.WriteLine($"AccountStatus migration warning: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Migrates the Attachments.FilePath column from TEXT(255) to MEMO if needed.
        /// Old databases had TEXT(255) which truncates long absolute paths and causes insert errors.
        /// MS Access does not support ALTER COLUMN directly, so we use a workaround:
        /// add a new MEMO column, copy data, drop old column, rename new column.
        /// </summary>
        private static void MigrateAttachmentFilePath(OleDbConnection connection)
        {
            try
            {
                // Test if FilePath can hold a long string — if it's TEXT(255) this will fail
                // We detect this by checking the column type via schema
                using var schemaCmd = new OleDbCommand(
                    "SELECT TOP 1 FilePath FROM Attachments", connection);
                using var reader = schemaCmd.ExecuteReader(System.Data.CommandBehavior.SchemaOnly);
                var schema = reader.GetSchemaTable();

                if (schema == null) return;

                foreach (System.Data.DataRow row in schema.Rows)
                {
                    if (row["ColumnName"]?.ToString() == "FilePath")
                    {
                        // ProviderType 130 = MEMO/LongText in ACE OleDb
                        // ProviderType 202 = TEXT (short) in ACE OleDb
                        int providerType = Convert.ToInt32(row["ProviderType"]);
                        if (providerType != 130) // Not MEMO — needs migration
                        {
                            reader.Close();
                            UpgradeFilePathToMemo(connection);
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MigrateAttachmentFilePath check failed: {ex.Message}");
            }
        }

        private static void UpgradeFilePathToMemo(OleDbConnection connection)
        {
            try
            {
                // Step 1: Add a new MEMO column
                using (var cmd = new OleDbCommand(
                    "ALTER TABLE Attachments ADD COLUMN FilePathMemo MEMO", connection))
                    cmd.ExecuteNonQuery();

                // Step 2: Copy existing data
                using (var cmd = new OleDbCommand(
                    "UPDATE Attachments SET FilePathMemo = FilePath", connection))
                    cmd.ExecuteNonQuery();

                // Step 3: Drop old TEXT column
                using (var cmd = new OleDbCommand(
                    "ALTER TABLE Attachments DROP COLUMN FilePath", connection))
                    cmd.ExecuteNonQuery();

                // Step 4: Rename new column — MS Access doesn't support RENAME COLUMN,
                // so we add FilePath as MEMO and copy again
                using (var cmd = new OleDbCommand(
                    "ALTER TABLE Attachments ADD COLUMN FilePath MEMO", connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new OleDbCommand(
                    "UPDATE Attachments SET FilePath = FilePathMemo", connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new OleDbCommand(
                    "ALTER TABLE Attachments DROP COLUMN FilePathMemo", connection))
                    cmd.ExecuteNonQuery();

                System.Diagnostics.Debug.WriteLine("Attachments.FilePath successfully migrated to MEMO.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpgradeFilePathToMemo failed: {ex.Message}");
            }
        }

        private static void SeedDepartments(OleDbConnection connection)
        {
            // Check if departments have already been seeded
            using (var checkCmd = new OleDbCommand("SELECT COUNT(*) FROM Departments", connection))
            {
                var count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                    return; // Already seeded
            }

            var departments = new[]
            {
                ("Human Resources", "Manages employee relations, recruitment, and HR policies"),
                ("Accounting",       "Handles financial records, payroll, and budgeting"),
                ("Sales",            "Manages client relationships and sales operations"),
                ("Operations",       "Oversees day-to-day business operations"),
                ("Registrar",        "Manages records, enrollment, and documentation"),
                ("IT Department",    "Provides technical support and manages IT infrastructure")
            };

            foreach (var (name, description) in departments)
            {
                using var cmd = new OleDbCommand(
                    "INSERT INTO Departments (DepartmentName, Description) VALUES (?, ?)",
                    connection);
                cmd.Parameters.AddWithValue("DepartmentName", name);
                cmd.Parameters.AddWithValue("Description", description);
                cmd.ExecuteNonQuery();
            }
        }
        private static void SeedAdminUser(OleDbConnection connection)
        {
            // Check if admin user already exists
            using (var checkCmd = new OleDbCommand("SELECT COUNT(*) FROM Users WHERE Username = ?", connection))
            {
                checkCmd.Parameters.AddWithValue("Username", "admin");
                var count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                    return; // Already seeded
            }

            // Look up the IT Department ID dynamically
            int itDeptId;
            using (var deptCmd = new OleDbCommand(
                "SELECT DepartmentID FROM Departments WHERE DepartmentName = ?", connection))
            {
                deptCmd.Parameters.AddWithValue("DepartmentName", "IT Department");
                var result = deptCmd.ExecuteScalar();
                if (result == null)
                    throw new InvalidOperationException(
                        "IT Department not found. Ensure departments are seeded before admin user.");
                itDeptId = (int)result;
            }

            // Compute SHA256 hash: ITHelpdesk_admin123_Salt2024 → Base64
            string passwordHash;
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = "ITHelpdesk_admin123_Salt2024";
                var bytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hash = sha256.ComputeHash(bytes);
                passwordHash = Convert.ToBase64String(hash);
            }

            using var cmd = new OleDbCommand(
                "INSERT INTO Users (FullName, Username, PasswordHash, Role, DepartmentID, ContactNumber, CreatedAt) " +
                "VALUES (?, ?, ?, ?, ?, ?, ?)",
                connection);
            cmd.Parameters.AddWithValue("FullName", "Administrator");
            cmd.Parameters.AddWithValue("Username", "admin");
            cmd.Parameters.AddWithValue("PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("Role", "Administrator");
            cmd.Parameters.Add(new OleDbParameter("DepartmentID", OleDbType.Integer) { Value = itDeptId });
            cmd.Parameters.AddWithValue("ContactNumber", "");
            cmd.Parameters.Add(new OleDbParameter("CreatedAt", OleDbType.Date) { Value = DateTime.Now });
            cmd.ExecuteNonQuery();
        }
    }
}
