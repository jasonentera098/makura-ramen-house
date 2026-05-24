using System;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk
{
    /// <summary>
    /// Utility class to seed test tickets for testing the summary cards functionality
    /// </summary>
    public static class TestDataSeeder
    {
        /// <summary>
        /// Seeds test data for Employee Dashboard testing including employee user and tickets
        /// </summary>
        public static void SeedEmployeeDashboardTestData()
        {
            try
            {
                // Ensure database is initialized first
                DatabaseInitializer.Initialize(DatabaseConfig.DatabasePath);
                
                var userRepo = new UserRepository(DatabaseConfig.ConnectionString);
                var ticketRepo = new TicketRepository(DatabaseConfig.ConnectionString);
                
                // Create test employee user
                int employeeUserId = CreateTestEmployeeUser(userRepo);
                
                // Create test tickets for the employee
                CreateTestTicketsForEmployee(ticketRepo, employeeUserId);
                
                Console.WriteLine("Employee Dashboard test data seeded successfully!");
                Console.WriteLine($"Employee User ID: {employeeUserId}");
                Console.WriteLine("Login credentials: employee1 / password123");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding employee dashboard data: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error seeding employee dashboard data: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private static int CreateTestEmployeeUser(UserRepository userRepo)
        {
            // Check if employee user already exists
            var existingUser = userRepo.GetByUsernameAsync("employee1").Result;
            if (existingUser != null)
            {
                Console.WriteLine("Employee user already exists, using existing user.");
                return existingUser.UserID;
            }

            // Hash password using the same method as the system
            string passwordHash;
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = "ITHelpdesk_password123_Salt2024";
                var bytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hash = sha256.ComputeHash(bytes);
                passwordHash = Convert.ToBase64String(hash);
            }

            var employeeUser = new User
            {
                FullName = "John Employee",
                Username = "employee1",
                PasswordHash = passwordHash,
                Role = "Employee",
                DepartmentID = 1, // First department (Human Resources)
                ContactNumber = "555-0123",
                CreatedAt = DateTime.Now
            };

            return userRepo.InsertAsync(employeeUser).Result;
        }

        private static void CreateTestTicketsForEmployee(TicketRepository ticketRepo, int employeeUserId)
        {
            var testTickets = new[]
            {
                new Ticket
                {
                    Title = "Computer running slowly",
                    Description = "My computer has been running very slowly lately, especially when opening applications.",
                    Category = "Hardware",
                    Priority = "Medium",
                    Status = "Pending",
                    DateSubmitted = DateTime.Now.AddDays(-1),
                    SubmittedBy = employeeUserId,
                    DepartmentID = 1
                },
                new Ticket
                {
                    Title = "Cannot access shared drive",
                    Description = "I'm unable to access the shared network drive that contains our department files.",
                    Category = "Network",
                    Priority = "High",
                    Status = "Pending",
                    DateSubmitted = DateTime.Now.AddDays(-2),
                    SubmittedBy = employeeUserId,
                    DepartmentID = 1
                },
                new Ticket
                {
                    Title = "Email attachment issues",
                    Description = "Having trouble opening email attachments in Outlook.",
                    Category = "Software",
                    Priority = "Low",
                    Status = "In Progress",
                    DateSubmitted = DateTime.Now.AddDays(-3),
                    SubmittedBy = employeeUserId,
                    DepartmentID = 1
                },
                new Ticket
                {
                    Title = "Printer not responding",
                    Description = "The department printer is not responding when I try to print documents.",
                    Category = "Hardware",
                    Priority = "Medium",
                    Status = "Resolved",
                    DateSubmitted = DateTime.Now.AddDays(-5),
                    DateResolved = DateTime.Now.AddDays(-1),
                    SubmittedBy = employeeUserId,
                    DepartmentID = 1
                },
                new Ticket
                {
                    Title = "Software license request",
                    Description = "Need a license for Microsoft Project for upcoming project management tasks.",
                    Category = "Software",
                    Priority = "Low",
                    Status = "Resolved",
                    DateSubmitted = DateTime.Now.AddDays(-7),
                    DateResolved = DateTime.Now.AddDays(-2),
                    SubmittedBy = employeeUserId,
                    DepartmentID = 1
                },
                new Ticket
                {
                    Title = "Password reset request",
                    Description = "Need help resetting my password for the HR system.",
                    Category = "Access",
                    Priority = "High",
                    Status = "Closed",
                    DateSubmitted = DateTime.Now.AddDays(-10),
                    DateResolved = DateTime.Now.AddDays(-8),
                    SubmittedBy = employeeUserId,
                    DepartmentID = 1
                }
            };

            foreach (var ticket in testTickets)
            {
                ticketRepo.InsertAsync(ticket).Wait();
            }
        }
        public static void SeedTestTickets()
        {
            try
            {
                // Ensure database is initialized first
                DatabaseInitializer.Initialize(DatabaseConfig.DatabasePath);
                
                var ticketRepo = new TicketRepository(DatabaseConfig.ConnectionString);
                
                // Create some test tickets with different statuses
                var testTickets = new[]
                {
                    new Ticket
                    {
                        Title = "Computer won't start",
                        Description = "My computer is not turning on when I press the power button.",
                        Category = "Hardware",
                        Priority = "High",
                        Status = "Pending",
                        DateSubmitted = DateTime.Now.AddDays(-2),
                        SubmittedBy = 1, // Admin user
                        DepartmentID = 1 // First department
                    },
                    new Ticket
                    {
                        Title = "Email not working",
                        Description = "Cannot send or receive emails in Outlook.",
                        Category = "Software",
                        Priority = "Medium",
                        Status = "In Progress",
                        DateSubmitted = DateTime.Now.AddDays(-1),
                        SubmittedBy = 1,
                        DepartmentID = 1
                    },
                    new Ticket
                    {
                        Title = "Printer offline",
                        Description = "Office printer shows as offline and won't print documents.",
                        Category = "Hardware",
                        Priority = "Low",
                        Status = "Resolved",
                        DateSubmitted = DateTime.Now.AddDays(-3),
                        DateResolved = DateTime.Now.AddHours(-2),
                        SubmittedBy = 1,
                        DepartmentID = 1
                    },
                    new Ticket
                    {
                        Title = "Network connection issues",
                        Description = "Internet connection is very slow and keeps dropping.",
                        Category = "Network",
                        Priority = "High",
                        Status = "Closed",
                        DateSubmitted = DateTime.Now.AddDays(-5),
                        DateResolved = DateTime.Now.AddDays(-1),
                        SubmittedBy = 1,
                        DepartmentID = 1
                    },
                    new Ticket
                    {
                        Title = "Software installation request",
                        Description = "Need Adobe Acrobat installed on my workstation.",
                        Category = "Software",
                        Priority = "Low",
                        Status = "Pending",
                        DateSubmitted = DateTime.Now,
                        SubmittedBy = 1,
                        DepartmentID = 1
                    }
                };

                foreach (var ticket in testTickets)
                {
                    ticketRepo.InsertAsync(ticket).Wait();
                }

                Console.WriteLine("Test tickets seeded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding dashboard data: {ex.Message}");
                // Also write to debug output for development
                System.Diagnostics.Debug.WriteLine($"Error seeding dashboard data: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}