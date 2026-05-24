using System;
using System.Threading.Tasks;
using IT_Helpdesk.Tests;

namespace IT_Helpdesk
{
    /// <summary>
    /// Simple test runner to execute functionality tests
    /// </summary>
    public class TestRunner
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("IT Helpdesk - Functionality Tests");
            Console.WriteLine("==================================\n");
            
            // Check if user wants to seed employee test data
            if (args.Length > 0 && args[0] == "seed-employee")
            {
                Console.WriteLine("Seeding Employee Dashboard Test Data...");
                TestDataSeeder.SeedEmployeeDashboardTestData();
                Console.WriteLine("\nEmployee test data seeded successfully!");
                Console.WriteLine("Login with: employee1 / password123");
                return;
            }
            
            // Check if user wants to run Task 13.3 tests
            if (args.Length > 0 && args[0] == "task13.3")
            {
                await RunTask13_3Test.RunTestAsync();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }
            
            // Check if user wants to run Task 13.4 tests
            if (args.Length > 0 && args[0] == "task13.4")
            {
                await Task13_4Test.RunAsync();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }
            
            // Run user deletion tests (Task 9.4)
            await UserDeletionTests.RunAllTests();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}