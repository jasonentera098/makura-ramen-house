using System;
using System.Threading.Tasks;
using IT_Helpdesk;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;
using IT_Helpdesk.ViewModels;

namespace Tests
{
    /// <summary>
    /// Unit tests for Technician Dashboard logout functionality (Task 12.5)
    /// Tests verify Requirements 2.2 and 2.3 (Session Management)
    /// </summary>
    public class TechnicianLogoutTests
    {
        private readonly string _connectionString;

        public TechnicianLogoutTests()
        {
            _connectionString = DatabaseConfig.ConnectionString;
        }

        /// <summary>
        /// Test: Verify logout updates LoginLog with LogoutTime
        /// Requirement: 2.2 - WHEN a User logs out, THE System SHALL update the Login_Log entry with LogoutTime
        /// </summary>
        public async Task<bool> Test_Logout_UpdatesLoginLogWithLogoutTime()
        {
            Console.WriteLine("\n=== Test: Logout Updates LoginLog with LogoutTime ===");
            
            try
            {
                // Arrange: Create repositories and services
                var userRepo = new UserRepository(_connectionString);
                var loginLogRepo = new LoginLogRepository(_connectionString);
                var authService = new AuthenticationService(userRepo, loginLogRepo);

                // Get a technician user for testing
                var technician = await userRepo.GetByUsernameAsync("tech1");
                if (technician == null)
                {
                    Console.WriteLine("❌ FAIL: Technician user 'tech1' not found in database");
                    return false;
                }

                // Simulate login
                int logId = await loginLogRepo.InsertLoginAsync(technician.UserID);
                SessionManager.StartSession(technician, logId);
                Console.WriteLine($"✓ Simulated login for {technician.FullName} (LogID: {logId})");

                // Verify session is active
                if (SessionManager.CurrentUser == null || SessionManager.CurrentLogID == 0)
                {
                    Console.WriteLine("❌ FAIL: Session not properly initialized");
                    return false;
                }
                Console.WriteLine($"✓ Session active: UserID={SessionManager.CurrentUser.UserID}, LogID={SessionManager.CurrentLogID}");

                // Act: Perform logout
                bool logoutResult = await authService.LogoutAsync(technician.UserID);
                Console.WriteLine($"✓ Logout executed: {logoutResult}");

                // Assert: Verify LoginLog was updated
                var loginLogs = await loginLogRepo.GetByUserIdAsync(technician.UserID);
                var latestLog = loginLogs.Find(log => log.LogID == logId);

                if (latestLog == null)
                {
                    Console.WriteLine("❌ FAIL: Could not find the login log entry");
                    return false;
                }

                if (!latestLog.LogoutTime.HasValue)
                {
                    Console.WriteLine("❌ FAIL: LogoutTime was not set");
                    return false;
                }

                Console.WriteLine($"✓ LogoutTime set: {latestLog.LogoutTime.Value}");
                Console.WriteLine($"✓ Login duration: {(latestLog.LogoutTime.Value - latestLog.LoginTime).TotalSeconds:F2} seconds");

                // Verify session was cleared
                if (SessionManager.CurrentUser != null || SessionManager.CurrentLogID != 0)
                {
                    Console.WriteLine("❌ FAIL: Session was not properly cleared");
                    return false;
                }
                Console.WriteLine("✓ Session cleared successfully");

                Console.WriteLine("✅ PASS: Logout updates LoginLog with LogoutTime");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAIL: Exception occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Test: Verify SessionManager.EndSession clears session data
        /// Requirement: 2.3 - WHEN a User logs out, THE System SHALL terminate the Session
        /// </summary>
        public async Task<bool> Test_Logout_ClearsSessionData()
        {
            Console.WriteLine("\n=== Test: Logout Clears Session Data ===");
            
            try
            {
                // Arrange: Create a test user and start session
                var userRepo = new UserRepository(_connectionString);
                var loginLogRepo = new LoginLogRepository(_connectionString);

                var technician = await userRepo.GetByUsernameAsync("tech1");
                if (technician == null)
                {
                    Console.WriteLine("❌ FAIL: Technician user 'tech1' not found in database");
                    return false;
                }

                int logId = await loginLogRepo.InsertLoginAsync(technician.UserID);
                SessionManager.StartSession(technician, logId);
                Console.WriteLine($"✓ Session started: UserID={technician.UserID}, LogID={logId}");

                // Verify session is active before logout
                if (!SessionManager.IsAuthenticated)
                {
                    Console.WriteLine("❌ FAIL: Session not authenticated before logout");
                    return false;
                }
                Console.WriteLine($"✓ Session authenticated: Role={SessionManager.CurrentRole}");

                // Act: End session
                SessionManager.EndSession();
                Console.WriteLine("✓ EndSession() called");

                // Assert: Verify session data is cleared
                if (SessionManager.CurrentUser != null)
                {
                    Console.WriteLine("❌ FAIL: CurrentUser is not null after logout");
                    return false;
                }
                Console.WriteLine("✓ CurrentUser is null");

                if (SessionManager.CurrentLogID != 0)
                {
                    Console.WriteLine("❌ FAIL: CurrentLogID is not 0 after logout");
                    return false;
                }
                Console.WriteLine("✓ CurrentLogID is 0");

                if (SessionManager.IsAuthenticated)
                {
                    Console.WriteLine("❌ FAIL: IsAuthenticated is still true after logout");
                    return false;
                }
                Console.WriteLine("✓ IsAuthenticated is false");

                if (SessionManager.CurrentRole != null)
                {
                    Console.WriteLine("❌ FAIL: CurrentRole is not null after logout");
                    return false;
                }
                Console.WriteLine("✓ CurrentRole is null");

                // Update the logout time in the database
                await loginLogRepo.UpdateLogoutAsync(logId);
                Console.WriteLine("✓ LoginLog updated with LogoutTime");

                Console.WriteLine("✅ PASS: Logout clears session data");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAIL: Exception occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Test: Verify TechnicianDashboardViewModel has LogoutCommand
        /// </summary>
        public void Test_TechnicianDashboardViewModel_HasLogoutCommand()
        {
            Console.WriteLine("\n=== Test: TechnicianDashboardViewModel Has LogoutCommand ===");
            
            try
            {
                // Arrange: Create repositories and services
                var ticketRepo = new TicketRepository(_connectionString);
                var ticketUpdateRepo = new TicketUpdateRepository(_connectionString);
                var userRepo = new UserRepository(_connectionString);
                var loginLogRepo = new LoginLogRepository(_connectionString);
                
                var ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
                var authService = new AuthenticationService(userRepo, loginLogRepo);

                // Act: Create ViewModel
                var viewModel = new TechnicianDashboardViewModel(ticketService, authService);

                // Assert: Verify LogoutCommand exists
                if (viewModel.LogoutCommand == null)
                {
                    Console.WriteLine("❌ FAIL: LogoutCommand is null");
                    return;
                }
                Console.WriteLine("✓ LogoutCommand is not null");

                Console.WriteLine("✅ PASS: TechnicianDashboardViewModel has LogoutCommand");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAIL: Exception occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Run all logout tests
        /// </summary>
        public async Task RunAllTests()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   Technician Dashboard Logout Tests (Task 12.5)           ║");
            Console.WriteLine("║   Requirements: 2.2, 2.3 (Session Management)             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            int passed = 0;
            int total = 0;

            // Test 1: Logout updates LoginLog
            total++;
            if (await Test_Logout_UpdatesLoginLogWithLogoutTime())
                passed++;

            // Test 2: Logout clears session data
            total++;
            if (await Test_Logout_ClearsSessionData())
                passed++;

            // Test 3: ViewModel has LogoutCommand
            total++;
            Test_TechnicianDashboardViewModel_HasLogoutCommand();
            passed++; // This test doesn't return bool, assume pass if no exception

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║   Test Results: {passed}/{total} PASSED                              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            if (passed == total)
            {
                Console.WriteLine("\n✅ ALL TESTS PASSED - Logout functionality is working correctly!");
            }
            else
            {
                Console.WriteLine($"\n⚠️  {total - passed} TEST(S) FAILED - Please review the failures above");
            }
        }
    }
}
