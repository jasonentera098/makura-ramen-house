using System;
using System.Threading.Tasks;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;

namespace IT_Helpdesk.Tests
{
    /// <summary>
    /// Tests for Task 11.6: Auto-set Status, DateSubmitted, SubmittedBy, DepartmentID
    /// </summary>
    public class TicketAutoSetTests
    {
        private readonly string _connectionString;
        private readonly ITicketService _ticketService;

        public TicketAutoSetTests()
        {
            _connectionString = DatabaseConfig.ConnectionString;
            var ticketRepo = new TicketRepository(_connectionString);
            var ticketUpdateRepo = new TicketUpdateRepository(_connectionString);
            var userRepo = new UserRepository(_connectionString);
            _ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
        }

        /// <summary>
        /// Test: Verify that Status, DateSubmitted, SubmittedBy, and DepartmentID are auto-set
        /// Validates Task 11.6
        /// </summary>
        public async Task<bool> TestAutoSetFields()
        {
            Console.WriteLine("\n[TEST] Auto-set Status, DateSubmitted, SubmittedBy, DepartmentID");
            Console.WriteLine("------------------------------------------------------------------");

            try
            {
                // Arrange: Create a ticket with only user-provided data
                var ticket = new Ticket
                {
                    Title = "Test Ticket for Auto-Set",
                    Description = "Testing that system fields are automatically set",
                    Category = "Software",
                    Priority = "Medium"
                    // Status, DateSubmitted, SubmittedBy, DepartmentID should NOT be set here
                };

                int testUserId = 1; // Admin user
                int testDepartmentId = 1; // First department
                DateTime beforeSubmit = DateTime.Now;

                // Act: Create the ticket through the service
                int ticketId = await _ticketService.CreateTicketAsync(ticket, testUserId, testDepartmentId);

                // Assert: Verify the ticket was created
                if (ticketId <= 0)
                {
                    Console.WriteLine("❌ FAIL: Ticket was not created");
                    return false;
                }

                // Retrieve the ticket to verify auto-set fields
                var createdTicket = await _ticketService.GetTicketByIdAsync(ticketId);
                if (createdTicket == null)
                {
                    Console.WriteLine("❌ FAIL: Could not retrieve created ticket");
                    return false;
                }

                // Verify Status is set to "Pending"
                if (createdTicket.Status != "Pending")
                {
                    Console.WriteLine($"❌ FAIL: Status should be 'Pending' but was '{createdTicket.Status}'");
                    return false;
                }
                Console.WriteLine($"✅ Status auto-set to: {createdTicket.Status}");

                // Verify DateSubmitted is set to current time
                if (createdTicket.DateSubmitted < beforeSubmit || createdTicket.DateSubmitted > DateTime.Now.AddSeconds(5))
                {
                    Console.WriteLine($"❌ FAIL: DateSubmitted is not within expected range");
                    return false;
                }
                Console.WriteLine($"✅ DateSubmitted auto-set to: {createdTicket.DateSubmitted}");

                // Verify SubmittedBy is set to the provided user ID
                if (createdTicket.SubmittedBy != testUserId)
                {
                    Console.WriteLine($"❌ FAIL: SubmittedBy should be {testUserId} but was {createdTicket.SubmittedBy}");
                    return false;
                }
                Console.WriteLine($"✅ SubmittedBy auto-set to: {createdTicket.SubmittedBy}");

                // Verify DepartmentID is set to the provided department ID
                if (createdTicket.DepartmentID != testDepartmentId)
                {
                    Console.WriteLine($"❌ FAIL: DepartmentID should be {testDepartmentId} but was {createdTicket.DepartmentID}");
                    return false;
                }
                Console.WriteLine($"✅ DepartmentID auto-set to: {createdTicket.DepartmentID}");

                // Verify AssignedTo is null (not assigned yet)
                if (createdTicket.AssignedTo != null)
                {
                    Console.WriteLine($"❌ FAIL: AssignedTo should be null but was {createdTicket.AssignedTo}");
                    return false;
                }
                Console.WriteLine($"✅ AssignedTo correctly set to: null");

                // Verify DateResolved is null (not resolved yet)
                if (createdTicket.DateResolved != null)
                {
                    Console.WriteLine($"❌ FAIL: DateResolved should be null but was {createdTicket.DateResolved}");
                    return false;
                }
                Console.WriteLine($"✅ DateResolved correctly set to: null");

                Console.WriteLine($"\n✅ PASS: All fields auto-set correctly for Ticket #{ticketId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAIL: Unexpected exception - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Run all ticket auto-set tests
        /// </summary>
        public static async Task RunAllTests()
        {
            var tests = new TicketAutoSetTests();
            int passed = 0;
            int total = 0;

            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  Ticket Auto-Set Tests (Task 11.6)                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");

            // Test: Auto-set fields
            total++;
            if (await tests.TestAutoSetFields())
                passed++;

            // Summary
            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Test Results: {passed}/{total} passed                              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        }
    }
}
