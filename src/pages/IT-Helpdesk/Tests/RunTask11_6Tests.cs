using System;
using System.Threading.Tasks;
using IT_Helpdesk.Tests;

namespace IT_Helpdesk
{
    /// <summary>
    /// Test runner for Task 11.6 - Auto-set ticket fields
    /// </summary>
    public class Task11_6TestRunner
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Task 11.6: Auto-Set Ticket Fields Test Runner        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");

            try
            {
                // Ensure database is initialized
                DatabaseInitializer.Initialize(DatabaseConfig.DatabasePath);
                Console.WriteLine("✅ Database initialized successfully\n");

                // Run the auto-set tests
                await TicketAutoSetTests.RunAllTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Fatal error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
