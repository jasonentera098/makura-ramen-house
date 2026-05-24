using System.IO;
using System.Windows;
using System.Windows.Threading;
using IT_Helpdesk.Helpers;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        /// <summary>
        /// Handles any unhandled exceptions that bubble up to the application level.
        /// Database / LAN connection errors are shown with a specific, actionable message.
        /// All other errors fall back to a generic "contact IT support" message.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var ex = e.Exception;

            // Log the full exception details
            LogUnhandledException(ex);

            // ── Database / LAN connection errors ────────────────────────────
            if (ex is DatabaseConnectionException dbEx)
            {
                MessageBox.Show(
                    dbEx.Message,
                    "Database Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                e.Handled = true;
                return;
            }

            // Check if the inner exception is a database connection error
            // (e.g. wrapped by async machinery or a service layer)
            if (ex.InnerException is DatabaseConnectionException innerDbEx)
            {
                MessageBox.Show(
                    innerDbEx.Message,
                    "Database Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                e.Handled = true;
                return;
            }

            // ── Generic fallback ─────────────────────────────────────────────
            MessageBox.Show(
                $"An unexpected error occurred. Please contact IT support.\n\nDetails: {ex.Message}",
                "Unexpected Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Mark as handled to prevent application crash
            e.Handled = true;
        }

        /// <summary>
        /// Writes exception details to a log file and debug output.
        /// </summary>
        private static void LogUnhandledException(Exception ex)
        {
            try
            {
                string logDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "IT-Helpdesk",
                    "Logs");

                Directory.CreateDirectory(logDir);

                string logFile = Path.Combine(logDir, $"error_{DateTime.Now:yyyyMMdd}.log");
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] UNHANDLED EXCEPTION{Environment.NewLine}" +
                                  $"Message: {ex.Message}{Environment.NewLine}" +
                                  $"Type: {ex.GetType().FullName}{Environment.NewLine}" +
                                  $"Stack Trace:{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}" +
                                  $"{new string('-', 80)}{Environment.NewLine}";

                File.AppendAllText(logFile, logEntry);
            }
            catch
            {
                // If logging itself fails, fall back to debug output only
            }

            System.Diagnostics.Debug.WriteLine($"[UNHANDLED EXCEPTION] {ex.GetType().FullName}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                // Resolve the database path from appsettings.ini (falls back to local default)
                string dbPath = AppSettings.DatabasePath;
                System.Diagnostics.Debug.WriteLine($"Database path: {dbPath}");

                // ── Startup connectivity check ───────────────────────────────
                // Verify the database is reachable before attempting any operations.
                // For a fresh install the file may not exist yet, so we only block
                // startup if the file exists but cannot be opened (network issue, driver
                // missing, locked, etc.).  DatabaseInitializer will create the file when
                // it is absent.
                if (File.Exists(dbPath) || dbPath.StartsWith(@"\\"))
                {
                    var (connected, connectionError) = DatabaseConfig.TestConnection();
                    if (!connected)
                    {
                        System.Diagnostics.Debug.WriteLine($"[App.OnStartup] Database connectivity check failed: {connectionError}");

                        MessageBox.Show(
                            connectionError ??
                            "Unable to connect to the helpdesk database on the network. " +
                            "Please ensure you are connected to the LAN and the database server is accessible.",
                            "Database Connection Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        Shutdown(1);
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine("Database connectivity check passed.");
                }

                // Create the database file and all tables if they don't already exist
                DatabaseInitializer.Initialize(dbPath);
                System.Diagnostics.Debug.WriteLine("Database initialized successfully");

                // Seed test data for demonstration (only if no tickets exist)
                try
                {
                    var ticketRepo = new TicketRepository(DatabaseConfig.ConnectionString);
                    var existingTickets = ticketRepo.GetAllAsync().Result;
                    if (existingTickets.Count == 0)
                    {
                        TestDataSeeder.SeedTestTickets();
                        System.Diagnostics.Debug.WriteLine("Test data seeded successfully");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Found {existingTickets.Count} existing tickets, skipping test data seeding");
                    }
                }
                catch (Exception ex)
                {
                    // Log error but don't prevent app startup
                    System.Diagnostics.Debug.WriteLine($"Error seeding test data: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                }

                // Wire up services and set LoginViewModel as DataContext for MainWindow
                var userRepo = new UserRepository(DatabaseConfig.ConnectionString);
                var loginLogRepo = new LoginLogRepository(DatabaseConfig.ConnectionString);
                var authService = new AuthenticationService(userRepo, loginLogRepo);
                var loginViewModel = new LoginViewModel(authService);

                var loginWindow = new MainWindow();
                loginWindow.DataContext = loginViewModel;
                loginWindow.Show();
                
                System.Diagnostics.Debug.WriteLine("Application startup completed successfully");
            }
            catch (DatabaseConnectionException dbEx)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection error during startup: {dbEx.Message}");

                MessageBox.Show(
                    dbEx.Message,
                    "Database Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                Shutdown(1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Critical error during application startup: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Show error to user
                MessageBox.Show($"Application failed to start: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Shutdown the application
                Shutdown(1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application is shutting down...");
            
            // Perform any cleanup here if needed
            // For example: close database connections, save settings, etc.
            
            base.OnExit(e);
            
            System.Diagnostics.Debug.WriteLine("Application shutdown complete.");
        }
    }
}
