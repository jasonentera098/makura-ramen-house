using System;
using System.Threading.Tasks;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;

namespace IT_Helpdesk.Tests
{
    /// <summary>
    /// Tests for user deletion with ticket dependency validation (Requirement 30.6)
    /// </summary>
    public class UserDeletionTests
    {
        private readonly string _connectionString;
        private readonly IUserService _userService;
        private readonly ITicketRepository _ticketRepository;

        public UserDeletionTests()
        {
            _connectionString = DatabaseConfig.ConnectionString;
            var userRepo = new UserRepository(_connectionString);
            _ticketRepository = new TicketRepository(_connectionString);
            _userService = new UserService(userRepo, _ticketRepository);
        }

        /// <summary>
        /// Test: User with submitted tickets cannot be deleted
        /// Validates Requirement 30.6
        /// </summary>
        public async Task<bool> TestDeleteUserWithSubmittedTickets()
        {
            Console.WriteLine("\n[TEST] Delete User with Submitted Tickets");
            Console.WriteLine("------------------------------------------");

            try
            {
                // Find a user who has submitted tickets
                var tickets = await _ticketRepository.GetAllAsync();
                if (tickets.Count == 0)
                {
                    Console.WriteLine("❌ SKIP: No tickets in database to test with");
                    return false;
                }

                var testTicket = tickets[0];
                var userId = testTicket.SubmittedBy;

                Console.WriteLine($"Attempting to delete user ID {userId} who submitted ticket #{testTicket.TicketID}");

                // Attempt to delete the user - should throw InvalidOperationException
                await _userService.DeleteUserAsync(userId);

                Console.WriteLine("❌ FAIL: Expected InvalidOperationException but deletion succeeded");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("submitted") && ex.Message.Contains("ticket"))
                {
                    Console.WriteLine($"✅ PASS: Deletion correctly blocked - {ex.Message}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ FAIL: Wrong exception message - {ex.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAIL: Unexpected exception - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test: User with assigned tickets cannot be deleted
        /// Validates Requirement 30.6
        /// </summary>
        public async Task<bool> TestDeleteUserWithAssignedTickets()
        {
            Console.WriteLine("\n[TEST] Delete User with Assigned Tickets");
            Console.WriteLine("------------------------------------------");

            try
            {
                // Find a user who has been assigned tickets
                var tickets = await _ticketRepository.GetAllAsync();
                var assignedTicket = tickets.Find(t => t.AssignedTo.HasValue);

                if (assignedTicket == null)
                {
                    Console.WriteLine("❌ SKIP: No assigned tickets in database to test with");
                    return false;
                }

                var userId = assignedTicket.AssignedTo!.Value;

                Console.WriteLine($"Attempting to delete user ID {userId} who is assigned ticket #{assignedTicket.TicketID}");

                // Attempt to delete the user - should throw InvalidOperationException
                await _userService.DeleteUserAsync(userId);

                Console.WriteLine("❌ FAIL: Expected InvalidOperationException but deletion succeeded");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("assigned") && ex.Message.Contains("ticket"))
                {
                    Console.WriteLine($"✅ PASS: Deletion correctly blocked - {ex.Message}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ FAIL: Wrong exception message - {ex.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAIL: Unexpected exception - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Run all user deletion tests
        /// </summary>
        public static async Task RunAllTests()
        {
            var tests = new UserDeletionTests();
            int passed = 0;
            int total = 0;

            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  User Deletion Validation Tests (Task 9.4)            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");

            // Test 1: User with submitted tickets
            total++;
            if (await tests.TestDeleteUserWithSubmittedTickets())
                passed++;

            // Test 2: User with assigned tickets
            total++;
            if (await tests.TestDeleteUserWithAssignedTickets())
                passed++;

            // Summary
            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Test Results: {passed}/{total} passed                              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        }
    }
}
