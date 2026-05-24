using System;
using System.Threading.Tasks;
using IT_Helpdesk.Tests;

namespace IT_Helpdesk.Tests
{
    /// <summary>
    /// Console application entry point for running tests
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("IT Helpdesk - Test Suite");
            Console.WriteLine("========================\n");
            
            try
            {
                // Run user deletion tests (Task 9.4)
                await UserDeletionTests.RunAllTests();
                
                // Run ticket auto-set tests (Task 11.6)
                await TicketAutoSetTests.RunAllTests();
                
                // Run Task 13.4 test (DateResolved auto-set)
                Console.WriteLine("\n");
                bool task13_4Passed = await Task13_4Test.RunAsync();
                if (!task13_4Passed)
                {
                    Console.WriteLine("\n⚠️ Task 13.4 test failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Test execution failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
