using System;
using System.Linq;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;
using IT_Helpdesk.ViewModels;

namespace IT_Helpdesk.Tests
{
    /// <summary>
    /// Test runner for Task 13.2: Verify Status ComboBox is populated with valid next states
    /// </summary>
    public class RunTask13_2Test
    {
        public static void RunTest()
        {
            Console.WriteLine("=== Task 13.2: Status Lifecycle Test ===\n");

            try
            {
                // Setup
                var connStr = DatabaseConfig.ConnectionString;
                var ticketRepo = new TicketRepository(connStr);
                var ticketUpdateRepo = new TicketUpdateRepository(connStr);
                var userRepo = new UserRepository(connStr);
                var attachmentRepo = new AttachmentRepository(connStr);
                var ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
                var fileService = new FileService(attachmentRepo);

                // Test 1: Pending status should allow Pending and In Progress
                Console.WriteLine("Test 1: Pending Status");
                var pendingTicket = new Ticket
                {
                    TicketID = 1,
                    Title = "Test Ticket",
                    Description = "Test Description",
                    Status = "Pending",
                    Category = "Hardware",
                    Priority = "High",
                    DateSubmitted = DateTime.Now,
                    SubmittedBy = 1,
                    DepartmentID = 1
                };

                var viewModel1 = new TicketUpdateViewModel(ticketService, fileService, pendingTicket);
                Console.WriteLine($"  Current Status: {pendingTicket.Status}");
                Console.WriteLine($"  Available Statuses: {string.Join(", ", viewModel1.AvailableStatuses)}");
                
                bool test1Pass = viewModel1.AvailableStatuses.Contains("Pending") && 
                                 viewModel1.AvailableStatuses.Contains("In Progress") &&
                                 viewModel1.AvailableStatuses.Count == 2;
                Console.WriteLine($"  Result: {(test1Pass ? "PASS" : "FAIL")}");
                Console.WriteLine();

                // Test 2: In Progress status should allow In Progress and Resolved
                Console.WriteLine("Test 2: In Progress Status");
                var inProgressTicket = new Ticket
                {
                    TicketID = 2,
                    Title = "Test Ticket 2",
                    Description = "Test Description 2",
                    Status = "In Progress",
                    Category = "Software",
                    Priority = "Medium",
                    DateSubmitted = DateTime.Now,
                    SubmittedBy = 1,
                    DepartmentID = 1
                };

                var viewModel2 = new TicketUpdateViewModel(ticketService, fileService, inProgressTicket);
                Console.WriteLine($"  Current Status: {inProgressTicket.Status}");
                Console.WriteLine($"  Available Statuses: {string.Join(", ", viewModel2.AvailableStatuses)}");
                
                bool test2Pass = viewModel2.AvailableStatuses.Contains("In Progress") && 
                                 viewModel2.AvailableStatuses.Contains("Resolved") &&
                                 viewModel2.AvailableStatuses.Count == 2;
                Console.WriteLine($"  Result: {(test2Pass ? "PASS" : "FAIL")}");
                Console.WriteLine();

                // Test 3: Resolved status should only allow Resolved (technician cannot change)
                Console.WriteLine("Test 3: Resolved Status");
                var resolvedTicket = new Ticket
                {
                    TicketID = 3,
                    Title = "Test Ticket 3",
                    Description = "Test Description 3",
                    Status = "Resolved",
                    Category = "Network",
                    Priority = "Low",
                    DateSubmitted = DateTime.Now,
                    SubmittedBy = 1,
                    DepartmentID = 1,
                    DateResolved = DateTime.Now
                };

                var viewModel3 = new TicketUpdateViewModel(ticketService, fileService, resolvedTicket);
                Console.WriteLine($"  Current Status: {resolvedTicket.Status}");
                Console.WriteLine($"  Available Statuses: {string.Join(", ", viewModel3.AvailableStatuses)}");
                
                bool test3Pass = viewModel3.AvailableStatuses.Contains("Resolved") &&
                                 viewModel3.AvailableStatuses.Count == 1;
                Console.WriteLine($"  Result: {(test3Pass ? "PASS" : "FAIL")}");
                Console.WriteLine();

                // Test 4: Closed status should only allow Closed
                Console.WriteLine("Test 4: Closed Status");
                var closedTicket = new Ticket
                {
                    TicketID = 4,
                    Title = "Test Ticket 4",
                    Description = "Test Description 4",
                    Status = "Closed",
                    Category = "Access",
                    Priority = "High",
                    DateSubmitted = DateTime.Now,
                    SubmittedBy = 1,
                    DepartmentID = 1,
                    DateResolved = DateTime.Now
                };

                var viewModel4 = new TicketUpdateViewModel(ticketService, fileService, closedTicket);
                Console.WriteLine($"  Current Status: {closedTicket.Status}");
                Console.WriteLine($"  Available Statuses: {string.Join(", ", viewModel4.AvailableStatuses)}");
                
                bool test4Pass = viewModel4.AvailableStatuses.Contains("Closed") &&
                                 viewModel4.AvailableStatuses.Count == 1;
                Console.WriteLine($"  Result: {(test4Pass ? "PASS" : "FAIL")}");
                Console.WriteLine();

                // Summary
                Console.WriteLine("=== Test Summary ===");
                bool allPass = test1Pass && test2Pass && test3Pass && test4Pass;
                Console.WriteLine($"Overall Result: {(allPass ? "ALL TESTS PASSED" : "SOME TESTS FAILED")}");
                
                if (allPass)
                {
                    Console.WriteLine("\n✓ Task 13.2 Implementation Verified:");
                    Console.WriteLine("  - Pending → In Progress lifecycle enforced");
                    Console.WriteLine("  - In Progress → Resolved lifecycle enforced");
                    Console.WriteLine("  - Resolved status locked (technician cannot change)");
                    Console.WriteLine("  - Closed status locked");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
