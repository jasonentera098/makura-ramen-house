using System;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// Test runner for Task 12.5: Implement logout functionality for Technician Dashboard
    /// </summary>
    public class RunTask12_5Tests
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Task 12.5 Tests...\n");
            
            try
            {
                var tests = new TechnicianLogoutTests();
                await tests.RunAllTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ CRITICAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
