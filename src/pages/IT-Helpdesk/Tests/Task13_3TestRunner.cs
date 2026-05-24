using System;
using System.Threading.Tasks;

namespace IT_Helpdesk.Tests
{
    /// <summary>
    /// Standalone console test runner for Task 13.3
    /// Run with: dotnet run --project IT-Helpdesk.csproj
    /// </summary>
    class Task13_3TestRunner
    {
        static async Task Main(string[] args)
        {
            await RunTask13_3Test.RunTestAsync();
        }
    }
}
