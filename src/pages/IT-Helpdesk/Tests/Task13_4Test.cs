using System;
using System.Threading.Tasks;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;

namespace IT_Helpdesk.Tests
{
    /// <summary>
    /// Test for Task 13.4: Set DateResolved=Now when status changes to "Resolved"
    /// </summary>
    public class Task13_4Test
    {
        public static async Task<bool> RunAsync()
        {
            Console.WriteLine("=== Task 13.4 Test: Set DateResolved when status changes to Resolved ===\n");

            try
            {
                // Setup
                var connectionString = DatabaseConfig.ConnectionString;

                var userRepo = new UserRepository(connectionString);
                var ticketRepo = new TicketRepository(connectionString);
                var ticketUpdateRepo = new TicketUpdateRepository(connectionString);

                var ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);

                // Get a test user (technician)
                var technician = await userRepo.GetByUsernameAsync("tech1");
                if (technician == null)
                {
                    Console.WriteLine("❌ FAIL: Test technician 'tech1' not found");
                    return false;
                }

                // Get an employee for ticket submission
                var employee = await userRepo.GetByUsernameAsync("emp1");
                if (employee == null)
                {
                    Console.WriteLine("❌ FAIL: Test employee 'emp1' not found");
                    return false;
                }

                Console.WriteLine("Test 1: Create ticket and verify DateResolved is null");
                Console.WriteLine("--------------------------------------------------------");

                // Create a test ticket
                var testTicket = new Ticket
                {
                    Title = "Test Ticket for Task 13.4",
                    Description = "Testing DateResolved auto-set functionality",
                    Category = "Software",
                    Priority = "High"
                };

                int ticketId = await ticketService.CreateTicketAsync(testTicket, employee.UserID, employee.DepartmentID);
                Console.WriteLine($"✅ Created ticket #{ticketId}");

                // Verify DateResolved is null initially
                var createdTicket = await ticketService.GetTicketByIdAsync(ticketId);
                if (createdTicket == null)
                {
                    Console.WriteLine("❌ FAIL: Could not retrieve created ticket");
                    return false;
                }

                if (createdTicket.DateResolved != null)
                {
                    Console.WriteLine($"❌ FAIL: DateResolved should be null but was {createdTicket.DateResolved}");
                    return false;
                }
                Console.WriteLine("✅ DateResolved is null for new ticket\n");

                Console.WriteLine("Test 2: Assign ticket and move to In Progress");
                Console.WriteLine("-----------------------------------------------");

                // Assign ticket to technician
                bool assigned = await ticketService.AssignTicketAsync(ticketId, technician.UserID, technician.UserID);
                if (!assigned)
                {
                    Console.WriteLine("❌ FAIL: Could not assign ticket");
                    return false;
                }
                Console.WriteLine($"✅ Assigned ticket to {technician.FullName}");

                // Verify status is now "In Progress" and DateResolved is still null
                var assignedTicket = await ticketService.GetTicketByIdAsync(ticketId);
                if (assignedTicket == null || assignedTicket.Status != "In Progress")
                {
                    Console.WriteLine($"❌ FAIL: Ticket status should be 'In Progress' but was '{assignedTicket?.Status}'");
                    return false;
                }

                if (assignedTicket.DateResolved != null)
                {
                    Console.WriteLine($"❌ FAIL: DateResolved should still be null but was {assignedTicket.DateResolved}");
                    return false;
                }
                Console.WriteLine("✅ Status is 'In Progress' and DateResolved is still null\n");

                Console.WriteLine("Test 3: Update status to Resolved and verify DateResolved is set");
                Console.WriteLine("-------------------------------------------------------------------");

                // Record time before update
                DateTime beforeUpdate = DateTime.Now;

                // Update status to Resolved
                bool statusUpdated = await ticketService.UpdateStatusAsync(ticketId, "Resolved", technician.UserID);
                if (!statusUpdated)
                {
                    Console.WriteLine("❌ FAIL: Could not update status to Resolved");
                    return false;
                }
                Console.WriteLine("✅ Updated status to 'Resolved'");

                // Record time after update
                DateTime afterUpdate = DateTime.Now;

                // Verify DateResolved is now set
                var resolvedTicket = await ticketService.GetTicketByIdAsync(ticketId);
                if (resolvedTicket == null)
                {
                    Console.WriteLine("❌ FAIL: Could not retrieve resolved ticket");
                    return false;
                }

                if (resolvedTicket.Status != "Resolved")
                {
                    Console.WriteLine($"❌ FAIL: Status should be 'Resolved' but was '{resolvedTicket.Status}'");
                    return false;
                }
                Console.WriteLine($"✅ Status is 'Resolved'");

                if (resolvedTicket.DateResolved == null)
                {
                    Console.WriteLine("❌ FAIL: DateResolved should be set but was null");
                    return false;
                }
                Console.WriteLine($"✅ DateResolved is set to: {resolvedTicket.DateResolved}");

                // Verify DateResolved is within reasonable time range
                if (resolvedTicket.DateResolved < beforeUpdate || resolvedTicket.DateResolved > afterUpdate)
                {
                    Console.WriteLine($"❌ FAIL: DateResolved ({resolvedTicket.DateResolved}) is not within expected range ({beforeUpdate} - {afterUpdate})");
                    return false;
                }
                Console.WriteLine("✅ DateResolved timestamp is correct\n");

                Console.WriteLine("Test 4: Verify TicketUpdate record was created");
                Console.WriteLine("-----------------------------------------------");

                var updates = await ticketUpdateRepo.GetByTicketIdAsync(ticketId);
                bool hasResolvedUpdate = false;
                foreach (var update in updates)
                {
                    if (update.StatusChange != null && update.StatusChange.Contains("Resolved"))
                    {
                        hasResolvedUpdate = true;
                        Console.WriteLine($"✅ Found TicketUpdate record: {update.StatusChange}");
                        break;
                    }
                }

                if (!hasResolvedUpdate)
                {
                    Console.WriteLine("❌ FAIL: No TicketUpdate record found for status change to Resolved");
                    return false;
                }

                Console.WriteLine("\n=== ALL TESTS PASSED ===");
                Console.WriteLine("\nTask 13.4 Implementation Summary:");
                Console.WriteLine("- DateResolved is null when ticket is created");
                Console.WriteLine("- DateResolved remains null when ticket is assigned/in progress");
                Console.WriteLine("- DateResolved is automatically set to current timestamp when status changes to 'Resolved'");
                Console.WriteLine("- TicketUpdate record is created documenting the status change");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ EXCEPTION: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
