using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IT_Helpdesk.Services;
using ClosedXML.Excel;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace IT_Helpdesk.ViewModels
{
    public class AllTicketReportItem
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SubmittedByName { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = string.Empty;
        public string DateSubmitted { get; set; } = string.Empty;
        public string DateResolved { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class CategoryStatItem
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TechnicianPerformanceItem
    {
        public string FullName { get; set; } = string.Empty;
        public int Assigned { get; set; }
        public int Resolved { get; set; }
        public int Pending { get; set; }
        public string AverageResolutionTime { get; set; } = string.Empty;
    }

    public class DepartmentReportItem
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TicketCount { get; set; }
        public int PendingCount { get; set; }
        public int InProgressCount { get; set; }
        public int ResolvedCount { get; set; }
        public int ClosedCount { get; set; }
    }

    public class AdminReportViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;

        private DateTime? _fromDate;
        private DateTime? _toDate;
        private string _selectedCategory = "All";
        private string _selectedStatus = "All";
        private ObservableCollection<AllTicketReportItem> _allTickets = new();
        private int _pendingCount;
        private int _inProgressCount;
        private int _resolvedCount;
        private int _closedCount;
        private ObservableCollection<CategoryStatItem> _categoryStats = new();
        private ObservableCollection<TechnicianPerformanceItem> _technicianPerformance = new();
        private ObservableCollection<DepartmentReportItem> _departmentReport = new();
        private bool _isLoading;
        private string _reportSummary = string.Empty;

        public event EventHandler? PrintRequested;
        public event EventHandler? CloseRequested;

        public AdminReportViewModel(IReportService reportService, IUserService userService, IDepartmentService departmentService)
        {
            _reportService = reportService;
            _userService = userService;
            _departmentService = departmentService;

            GenerateCommand = new RelayCommand(async () => await GenerateAsync());
            PrintCommand = new RelayCommand(() => PrintRequested?.Invoke(this, EventArgs.Empty));
            ExportToExcelCommand = new RelayCommand(async () => await ExportToExcelAsync(), () => AllTickets.Count > 0);
            ExportToPdfCommand = new RelayCommand(async () => await ExportToPdfAsync(), () => !string.IsNullOrEmpty(ReportSummary));
            TodayCommand = new RelayCommand(async () => { SetToday(); await GenerateAsync(); });
            ThisWeekCommand = new RelayCommand(async () => { SetThisWeek(); await GenerateAsync(); });
            ThisMonthCommand = new RelayCommand(async () => { SetThisMonth(); await GenerateAsync(); });
            CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));

            // Configure QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;

            SetThisMonth();
            _ = GenerateAsync();
        }

        #region Properties

        public DateTime? FromDate
        {
            get => _fromDate;
            set => SetProperty(ref _fromDate, value);
        }

        public DateTime? ToDate
        {
            get => _toDate;
            set => SetProperty(ref _toDate, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public ObservableCollection<AllTicketReportItem> AllTickets
        {
            get => _allTickets;
            set => SetProperty(ref _allTickets, value);
        }

        public int PendingCount
        {
            get => _pendingCount;
            set => SetProperty(ref _pendingCount, value);
        }

        public int InProgressCount
        {
            get => _inProgressCount;
            set => SetProperty(ref _inProgressCount, value);
        }

        public int ResolvedCount
        {
            get => _resolvedCount;
            set => SetProperty(ref _resolvedCount, value);
        }

        public int ClosedCount
        {
            get => _closedCount;
            set => SetProperty(ref _closedCount, value);
        }

        public ObservableCollection<CategoryStatItem> CategoryStats
        {
            get => _categoryStats;
            set => SetProperty(ref _categoryStats, value);
        }

        public ObservableCollection<TechnicianPerformanceItem> TechnicianPerformance
        {
            get => _technicianPerformance;
            set => SetProperty(ref _technicianPerformance, value);
        }

        public ObservableCollection<DepartmentReportItem> DepartmentReport
        {
            get => _departmentReport;
            set => SetProperty(ref _departmentReport, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ReportSummary
        {
            get => _reportSummary;
            set => SetProperty(ref _reportSummary, value);
        }

        #endregion

        #region Commands

        public ICommand GenerateCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand ExportToPdfCommand { get; }
        public ICommand TodayCommand { get; }
        public ICommand ThisWeekCommand { get; }
        public ICommand ThisMonthCommand { get; }
        public ICommand CloseCommand { get; }

        #endregion

        #region Methods

        private void SetToday()
        {
            FromDate = DateTime.Today;
            ToDate = DateTime.Today;
        }

        private void SetThisWeek()
        {
            var today = DateTime.Today;
            FromDate = today.AddDays(-(int)today.DayOfWeek);
            ToDate = today;
        }

        private void SetThisMonth()
        {
            var today = DateTime.Today;
            FromDate = new DateTime(today.Year, today.Month, 1);
            ToDate = today;
        }

        private async Task GenerateAsync()
        {
            try
            {
                IsLoading = true;

                string? category = SelectedCategory == "All" ? null : SelectedCategory;
                string? status = SelectedStatus == "All" ? null : SelectedStatus;

                // Load all tickets
                var tickets = await _reportService.GetTicketsByFiltersAsync(
                    FromDate, ToDate, category, status, null, null, null, "Administrator");

                // Cache user and department lookups
                var userCache = new Dictionary<int, string>();
                var departmentCache = new Dictionary<int, string>();

                async Task<string> GetUserName(int userId)
                {
                    if (userCache.TryGetValue(userId, out var name)) return name;
                    var user = await _userService.GetUserByIdAsync(userId);
                    name = user?.FullName ?? "Unknown";
                    userCache[userId] = name;
                    return name;
                }

                async Task<string> GetDepartmentName(int departmentId)
                {
                    if (departmentCache.TryGetValue(departmentId, out var name)) return name;
                    var department = await _departmentService.GetDepartmentByIdAsync(departmentId);
                    name = department?.DepartmentName ?? "Unknown";
                    departmentCache[departmentId] = name;
                    return name;
                }

                var items = new ObservableCollection<AllTicketReportItem>();
                foreach (var t in tickets)
                {
                    string submittedByName = await GetUserName(t.SubmittedBy);
                    string assignedToName = t.AssignedTo.HasValue
                        ? await GetUserName(t.AssignedTo.Value)
                        : "Unassigned";
                    string departmentName = await GetDepartmentName(t.DepartmentID);

                    items.Add(new AllTicketReportItem
                    {
                        TicketID = t.TicketID,
                        Title = t.Title,
                        Category = t.Category,
                        Priority = t.Priority,
                        Status = t.Status,
                        SubmittedByName = submittedByName,
                        AssignedToName = assignedToName,
                        DepartmentName = departmentName,
                        DateSubmitted = t.DateSubmitted.ToString("MM/dd/yyyy"),
                        DateResolved = t.DateResolved.HasValue ? t.DateResolved.Value.ToString("MM/dd/yyyy") : "-"
                    });
                }

                AllTickets = items;

                // Status summary counts
                PendingCount = tickets.Count(t => t.Status == "Pending");
                InProgressCount = tickets.Count(t => t.Status == "In Progress");
                ResolvedCount = tickets.Count(t => t.Status == "Resolved");
                ClosedCount = tickets.Count(t => t.Status == "Closed");

                // Category stats
                var allCategories = new[] { "Hardware", "Software", "Network", "Access", "Other" };
                var catStats = new ObservableCollection<CategoryStatItem>();
                foreach (var cat in allCategories)
                {
                    catStats.Add(new CategoryStatItem
                    {
                        Category = cat,
                        Count = tickets.Count(t => t.Category == cat)
                    });
                }
                CategoryStats = catStats;

                // Technician performance
                var perfData = await _reportService.GetTechnicianPerformanceAsync(FromDate, ToDate);
                var perfItems = new ObservableCollection<TechnicianPerformanceItem>();
                foreach (var p in perfData)
                {
                    // Format average resolution time
                    string avgResolutionTime = "-";
                    if (p.AverageResolutionTimeHours > 0)
                    {
                        if (p.AverageResolutionTimeHours < 24)
                        {
                            avgResolutionTime = $"{p.AverageResolutionTimeHours:F1}h";
                        }
                        else
                        {
                            double days = p.AverageResolutionTimeHours / 24;
                            avgResolutionTime = $"{days:F1}d";
                        }
                    }

                    perfItems.Add(new TechnicianPerformanceItem
                    {
                        FullName = p.FullName,
                        Assigned = p.Assigned,
                        Resolved = p.Resolved,
                        Pending = p.Pending,
                        AverageResolutionTime = avgResolutionTime
                    });
                }
                TechnicianPerformance = perfItems;

                // Department report - tickets by department
                await GenerateDepartmentReportAsync();

                // Generate intelligent summary and insights
                GenerateReportSummary();

                // Update export command availability
                ((RelayCommand)ExportToExcelCommand).RaiseCanExecuteChanged();
                ((RelayCommand)ExportToPdfCommand).RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GenerateDepartmentReportAsync()
        {
            try
            {
                // Get all departments
                var departments = await _departmentService.GetDepartmentsAsync();
                
                // Get all tickets for the date range (no other filters for department report)
                var allTickets = await _reportService.GetTicketsByFiltersAsync(
                    FromDate, ToDate, null, null, null, null, null, "Administrator");

                var departmentReportItems = new ObservableCollection<DepartmentReportItem>();

                foreach (var dept in departments)
                {
                    var deptTickets = allTickets.Where(t => t.DepartmentID == dept.DepartmentID).ToList();
                    
                    departmentReportItems.Add(new DepartmentReportItem
                    {
                        DepartmentName = dept.DepartmentName,
                        TicketCount = deptTickets.Count,
                        PendingCount = deptTickets.Count(t => t.Status == "Pending"),
                        InProgressCount = deptTickets.Count(t => t.Status == "In Progress"),
                        ResolvedCount = deptTickets.Count(t => t.Status == "Resolved"),
                        ClosedCount = deptTickets.Count(t => t.Status == "Closed")
                    });
                }

                // Sort by ticket count descending to show departments with most tickets first
                DepartmentReport = new ObservableCollection<DepartmentReportItem>(
                    departmentReportItems.OrderByDescending(d => d.TicketCount));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating department report: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateReportSummary()
        {
            try
            {
                if (AllTickets.Count == 0)
                {
                    ReportSummary = "No tickets found for the selected date range. Consider adjusting your filters to view data.";
                    return;
                }

                var summary = new System.Text.StringBuilder();
                int totalTickets = AllTickets.Count;

                // ── Overview Statement ──
                string dateRange = FromDate.HasValue && ToDate.HasValue
                    ? $"from {FromDate.Value:MMM dd, yyyy} to {ToDate.Value:MMM dd, yyyy}"
                    : "for the selected period";

                summary.AppendLine($"📊 Report Summary {dateRange}");
                summary.AppendLine();
                summary.AppendLine($"Total Tickets: {totalTickets}");
                summary.AppendLine();

                // ── Status Analysis ──
                summary.AppendLine("🔍 Key Findings:");
                summary.AppendLine();

                double resolvedRate = totalTickets > 0 ? (ResolvedCount + ClosedCount) * 100.0 / totalTickets : 0;
                double pendingRate = totalTickets > 0 ? PendingCount * 100.0 / totalTickets : 0;
                double inProgressRate = totalTickets > 0 ? InProgressCount * 100.0 / totalTickets : 0;

                summary.AppendLine($"• Resolution Rate: {resolvedRate:F1}% ({ResolvedCount + ClosedCount} of {totalTickets} tickets resolved/closed)");

                if (resolvedRate >= 80)
                    summary.AppendLine("  ✓ Excellent resolution performance");
                else if (resolvedRate >= 60)
                    summary.AppendLine("  ⚠ Good resolution rate, but room for improvement");
                else
                    summary.AppendLine("  ⚠ Low resolution rate - consider resource allocation review");

                summary.AppendLine();

                // ── Workload Distribution ──
                if (PendingCount > 0 || InProgressCount > 0)
                {
                    summary.AppendLine($"• Active Workload: {PendingCount + InProgressCount} tickets ({pendingRate + inProgressRate:F1}%)");
                    summary.AppendLine($"  - Pending: {PendingCount} ({pendingRate:F1}%)");
                    summary.AppendLine($"  - In Progress: {InProgressCount} ({inProgressRate:F1}%)");
                    summary.AppendLine();
                }

                // ── Category Analysis ──
                var topCategory = CategoryStats.OrderByDescending(c => c.Count).FirstOrDefault();
                if (topCategory != null && topCategory.Count > 0)
                {
                    double categoryPercent = totalTickets > 0 ? topCategory.Count * 100.0 / totalTickets : 0;
                    summary.AppendLine($"• Top Category: {topCategory.Category} ({topCategory.Count} tickets, {categoryPercent:F1}%)");
                    
                    if (categoryPercent > 40)
                        summary.AppendLine($"  ⚠ {topCategory.Category} issues dominate - consider specialized training or resources");
                    
                    summary.AppendLine();
                }

                // ── Department Analysis ──
                if (DepartmentReport.Count > 0)
                {
                    var topDept = DepartmentReport.OrderByDescending(d => d.TicketCount).FirstOrDefault();
                    if (topDept != null && topDept.TicketCount > 0)
                    {
                        double deptPercent = totalTickets > 0 ? topDept.TicketCount * 100.0 / totalTickets : 0;
                        summary.AppendLine($"• Highest Volume Department: {topDept.DepartmentName} ({topDept.TicketCount} tickets, {deptPercent:F1}%)");
                        
                        if (topDept.PendingCount > topDept.ResolvedCount + topDept.ClosedCount)
                            summary.AppendLine($"  ⚠ High pending count in {topDept.DepartmentName} - may need additional support");
                        
                        summary.AppendLine();
                    }
                }

                // ── Technician Performance Analysis ──
                if (TechnicianPerformance.Count > 0)
                {
                    var activeTechs = TechnicianPerformance.Where(t => t.Assigned > 0).ToList();
                    if (activeTechs.Count > 0)
                    {
                        double avgAssigned = activeTechs.Average(t => t.Assigned);
                        var topPerformer = activeTechs.OrderByDescending(t => t.Resolved).FirstOrDefault();
                        
                        summary.AppendLine($"• Technician Workload: {activeTechs.Count} active technicians, avg {avgAssigned:F1} tickets each");
                        
                        if (topPerformer != null)
                        {
                            double topPerformerRate = topPerformer.Assigned > 0 
                                ? topPerformer.Resolved * 100.0 / topPerformer.Assigned 
                                : 0;
                            summary.AppendLine($"  ✓ Top Performer: {topPerformer.FullName} ({topPerformer.Resolved} resolved, {topPerformerRate:F0}% rate)");
                        }

                        // Check for workload imbalance
                        var maxAssigned = activeTechs.Max(t => t.Assigned);
                        var minAssigned = activeTechs.Min(t => t.Assigned);
                        if (activeTechs.Count > 1 && maxAssigned > minAssigned * 2)
                        {
                            summary.AppendLine($"  ⚠ Workload imbalance detected (range: {minAssigned}-{maxAssigned} tickets)");
                        }

                        summary.AppendLine();
                    }
                }

                // ── Actionable Insights ──
                summary.AppendLine("💡 Recommendations:");
                summary.AppendLine();

                var insights = new List<string>();

                // Resolution rate insights
                if (resolvedRate < 70 && (PendingCount + InProgressCount) > totalTickets * 0.3)
                {
                    insights.Add("• Prioritize clearing the backlog of pending tickets to improve resolution rate");
                }

                // Category-specific insights
                if (topCategory != null)
                {
                    double categoryPercent = totalTickets > 0 ? topCategory.Count * 100.0 / totalTickets : 0;
                    if (categoryPercent > 40)
                    {
                        insights.Add($"• Develop specialized resources or documentation for {topCategory.Category} issues");
                    }
                }

                // Technician workload insights
                if (TechnicianPerformance.Count > 0)
                {
                    var activeTechs = TechnicianPerformance.Where(t => t.Assigned > 0).ToList();
                    if (activeTechs.Count > 1)
                    {
                        var maxAssigned = activeTechs.Max(t => t.Assigned);
                        var minAssigned = activeTechs.Min(t => t.Assigned);
                        if (maxAssigned > minAssigned * 2)
                        {
                            insights.Add("• Review ticket assignment process to balance workload across technicians");
                        }
                    }

                    // Check for technicians with high pending counts
                    var overloadedTechs = activeTechs.Where(t => t.Pending > t.Resolved && t.Pending > 5).ToList();
                    if (overloadedTechs.Count > 0)
                    {
                        insights.Add($"• {overloadedTechs.Count} technician(s) have high pending counts - consider redistributing work");
                    }
                }

                // Department insights
                if (DepartmentReport.Count > 0)
                {
                    var highPendingDepts = DepartmentReport
                        .Where(d => d.TicketCount > 0 && d.PendingCount > d.TicketCount * 0.5)
                        .ToList();
                    
                    if (highPendingDepts.Count > 0)
                    {
                        var deptNames = string.Join(", ", highPendingDepts.Select(d => d.DepartmentName));
                        insights.Add($"• Focus support on departments with high pending rates: {deptNames}");
                    }
                }

                // General insights
                if (InProgressCount > PendingCount && PendingCount > 0)
                {
                    insights.Add("• More tickets in progress than pending - good workflow, ensure timely completion");
                }

                if (insights.Count == 0)
                {
                    insights.Add("• System is performing well - maintain current processes and monitor trends");
                }

                foreach (var insight in insights)
                {
                    summary.AppendLine(insight);
                }

                ReportSummary = summary.ToString();
            }
            catch (Exception ex)
            {
                ReportSummary = $"Unable to generate summary: {ex.Message}";
            }
        }

        private async Task ExportToExcelAsync()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx",
                    FileName = $"IT_Helpdesk_Data_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                IsLoading = true;

                await Task.Run(() =>
                {
                    using var workbook = new XLWorkbook();
                    var sheet = workbook.Worksheets.Add("Helpdesk Report");
                    int currentRow = 1;

                    // HEADER
                    sheet.Cell(currentRow, 1).Value = "IT HELPDESK SYSTEM - DATA REPORT";
                    sheet.Range(currentRow, 1, currentRow, 10).Merge();
                    sheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    sheet.Cell(currentRow, 1).Style.Font.FontSize = 18;
                    sheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    sheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1A1A2E");
                    sheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.White;
                    currentRow += 2;

                    string dateRange = FromDate.HasValue && ToDate.HasValue
                        ? $"Report Period: {FromDate.Value:MMMM dd, yyyy} to {ToDate.Value:MMMM dd, yyyy}"
                        : "Report Period: All Time";
                    sheet.Cell(currentRow, 1).Value = dateRange;
                    sheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    currentRow++;

                    sheet.Cell(currentRow, 1).Value = $"Generated: {DateTime.Now:MMMM dd, yyyy hh:mm tt}";
                    sheet.Cell(currentRow, 1).Style.Font.Italic = true;
                    sheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Gray;
                    currentRow += 3;

                    // SECTION 1: ALL TICKETS
                    sheet.Cell(currentRow, 1).Value = "ALL TICKETS REPORT";
                    sheet.Range(currentRow, 1, currentRow, 10).Merge();
                    sheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    sheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                    sheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#007ACC");
                    sheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.White;
                    sheet.Row(currentRow).Height = 25;
                    currentRow += 2;

                    int headerRow = currentRow;
                    sheet.Cell(currentRow, 1).Value = "Ticket ID";
                    sheet.Cell(currentRow, 2).Value = "Title";
                    sheet.Cell(currentRow, 3).Value = "Category";
                    sheet.Cell(currentRow, 4).Value = "Priority";
                    sheet.Cell(currentRow, 5).Value = "Status";
                    sheet.Cell(currentRow, 6).Value = "Submitted By";
                    sheet.Cell(currentRow, 7).Value = "Department";
                    sheet.Cell(currentRow, 8).Value = "Assigned To";
                    sheet.Cell(currentRow, 9).Value = "Date Submitted";
                    sheet.Cell(currentRow, 10).Value = "Date Resolved";

                    var headerRange = sheet.Range(currentRow, 1, currentRow, 10);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4F8");
                    headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;

                    int dataStartRow = currentRow;
                    foreach (var ticket in AllTickets)
                    {
                        sheet.Cell(currentRow, 1).Value = ticket.TicketID;
                        sheet.Cell(currentRow, 2).Value = ticket.Title;
                        sheet.Cell(currentRow, 3).Value = ticket.Category;
                        sheet.Cell(currentRow, 4).Value = ticket.Priority;
                        sheet.Cell(currentRow, 5).Value = ticket.Status;
                        sheet.Cell(currentRow, 6).Value = ticket.SubmittedByName;
                        sheet.Cell(currentRow, 7).Value = ticket.DepartmentName;
                        sheet.Cell(currentRow, 8).Value = ticket.AssignedToName;
                        sheet.Cell(currentRow, 9).Value = ticket.DateSubmitted;
                        sheet.Cell(currentRow, 10).Value = ticket.DateResolved;

                        if ((currentRow - dataStartRow) % 2 == 1)
                            sheet.Range(currentRow, 1, currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#FAFAFA");

                        // Priority colors
                        var priorityCell = sheet.Cell(currentRow, 4);
                        priorityCell.Style.Font.Bold = true;
                        priorityCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        switch (ticket.Priority)
                        {
                            case "High":
                                priorityCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFEBEE");
                                priorityCell.Style.Font.FontColor = XLColor.FromHtml("#C62828");
                                break;
                            case "Medium":
                                priorityCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF3E0");
                                priorityCell.Style.Font.FontColor = XLColor.FromHtml("#EF6C00");
                                break;
                            case "Low":
                                priorityCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F5E9");
                                priorityCell.Style.Font.FontColor = XLColor.FromHtml("#2E7D32");
                                break;
                        }

                        // Status colors
                        var statusCell = sheet.Cell(currentRow, 5);
                        statusCell.Style.Font.Bold = true;
                        statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        switch (ticket.Status)
                        {
                            case "Pending":
                                statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F5F5F5");
                                statusCell.Style.Font.FontColor = XLColor.FromHtml("#757575");
                                break;
                            case "In Progress":
                                statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E3F2FD");
                                statusCell.Style.Font.FontColor = XLColor.FromHtml("#1976D2");
                                break;
                            case "Resolved":
                                statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F5E9");
                                statusCell.Style.Font.FontColor = XLColor.FromHtml("#388E3C");
                                break;
                            case "Closed":
                                statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#424242");
                                statusCell.Style.Font.FontColor = XLColor.White;
                                break;
                        }

                        currentRow++;
                    }

                    sheet.Range(headerRow, 1, currentRow - 1, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    currentRow += 3;

                    // SECTION 2: CATEGORY REPORT
                    sheet.Cell(currentRow, 1).Value = "TICKET CATEGORY REPORT";
                    sheet.Range(currentRow, 1, currentRow, 4).Merge();
                    sheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    sheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                    sheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#007ACC");
                    sheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.White;
                    sheet.Row(currentRow).Height = 25;
                    currentRow += 2;

                    headerRow = currentRow;
                    sheet.Cell(currentRow, 1).Value = "Category";
                    sheet.Cell(currentRow, 2).Value = "Count";
                    sheet.Range(currentRow, 1, currentRow, 2).Style.Font.Bold = true;
                    sheet.Range(currentRow, 1, currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4F8");
                    sheet.Range(currentRow, 1, currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;

                    dataStartRow = currentRow;
                    foreach (var cat in CategoryStats)
                    {
                        sheet.Cell(currentRow, 1).Value = cat.Category;
                        sheet.Cell(currentRow, 2).Value = cat.Count;
                        sheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        sheet.Cell(currentRow, 2).Style.Font.Bold = true;

                        if ((currentRow - dataStartRow) % 2 == 1)
                            sheet.Range(currentRow, 1, currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FAFAFA");

                        currentRow++;
                    }

                    sheet.Range(headerRow, 1, currentRow - 1, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    currentRow += 3;

                    // SECTION 3: DEPARTMENT REPORT
                    sheet.Cell(currentRow, 1).Value = "DEPARTMENT TICKET REPORT";
                    sheet.Range(currentRow, 1, currentRow, 6).Merge();
                    sheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    sheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                    sheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#007ACC");
                    sheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.White;
                    sheet.Row(currentRow).Height = 25;
                    currentRow += 2;

                    headerRow = currentRow;
                    sheet.Cell(currentRow, 1).Value = "Department";
                    sheet.Cell(currentRow, 2).Value = "Total";
                    sheet.Cell(currentRow, 3).Value = "Pending";
                    sheet.Cell(currentRow, 4).Value = "In Progress";
                    sheet.Cell(currentRow, 5).Value = "Resolved";
                    sheet.Cell(currentRow, 6).Value = "Closed";
                    sheet.Range(currentRow, 1, currentRow, 6).Style.Font.Bold = true;
                    sheet.Range(currentRow, 1, currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4F8");
                    sheet.Range(currentRow, 1, currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;

                    dataStartRow = currentRow;
                    foreach (var dept in DepartmentReport)
                    {
                        sheet.Cell(currentRow, 1).Value = dept.DepartmentName;
                        sheet.Cell(currentRow, 2).Value = dept.TicketCount;
                        sheet.Cell(currentRow, 3).Value = dept.PendingCount;
                        sheet.Cell(currentRow, 4).Value = dept.InProgressCount;
                        sheet.Cell(currentRow, 5).Value = dept.ResolvedCount;
                        sheet.Cell(currentRow, 6).Value = dept.ClosedCount;

                        for (int col = 2; col <= 6; col++)
                            sheet.Cell(currentRow, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        sheet.Cell(currentRow, 2).Style.Font.Bold = true;

                        if ((currentRow - dataStartRow) % 2 == 1)
                            sheet.Range(currentRow, 1, currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#FAFAFA");

                        currentRow++;
                    }

                    sheet.Range(headerRow, 1, currentRow - 1, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    currentRow += 3;

                    // SECTION 4: TECHNICIAN PERFORMANCE
                    sheet.Cell(currentRow, 1).Value = "TECHNICIAN PERFORMANCE REPORT";
                    sheet.Range(currentRow, 1, currentRow, 5).Merge();
                    sheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    sheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                    sheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#007ACC");
                    sheet.Cell(currentRow, 1).Style.Font.FontColor = XLColor.White;
                    sheet.Row(currentRow).Height = 25;
                    currentRow += 2;

                    headerRow = currentRow;
                    sheet.Cell(currentRow, 1).Value = "Technician Name";
                    sheet.Cell(currentRow, 2).Value = "Assigned";
                    sheet.Cell(currentRow, 3).Value = "Resolved";
                    sheet.Cell(currentRow, 4).Value = "Pending";
                    sheet.Cell(currentRow, 5).Value = "Avg Resolution Time";
                    sheet.Range(currentRow, 1, currentRow, 5).Style.Font.Bold = true;
                    sheet.Range(currentRow, 1, currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4F8");
                    sheet.Range(currentRow, 1, currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;

                    dataStartRow = currentRow;
                    foreach (var tech in TechnicianPerformance)
                    {
                        sheet.Cell(currentRow, 1).Value = tech.FullName;
                        sheet.Cell(currentRow, 2).Value = tech.Assigned;
                        sheet.Cell(currentRow, 3).Value = tech.Resolved;
                        sheet.Cell(currentRow, 4).Value = tech.Pending;
                        sheet.Cell(currentRow, 5).Value = tech.AverageResolutionTime;

                        for (int col = 2; col <= 5; col++)
                            sheet.Cell(currentRow, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        sheet.Cell(currentRow, 3).Style.Font.Bold = true;
                        sheet.Cell(currentRow, 3).Style.Font.FontColor = XLColor.FromHtml("#388E3C");

                        if ((currentRow - dataStartRow) % 2 == 1)
                            sheet.Range(currentRow, 1, currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#FAFAFA");

                        currentRow++;
                    }

                    sheet.Range(headerRow, 1, currentRow - 1, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    // Column widths
                    sheet.Column(1).Width = 12;
                    sheet.Column(2).Width = 35;
                    sheet.Column(3).Width = 12;
                    sheet.Column(4).Width = 12;
                    sheet.Column(5).Width = 14;
                    sheet.Column(6).Width = 18;
                    sheet.Column(7).Width = 18;
                    sheet.Column(8).Width = 18;
                    sheet.Column(9).Width = 15;
                    sheet.Column(10).Width = 15;

                    workbook.SaveAs(saveFileDialog.FileName);
                });

                MessageBox.Show($"Data report exported successfully to:\n{saveFileDialog.FileName}",
                    "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to Excel: {ex.Message}",
                    "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExportToPdfAsync()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    DefaultExt = "pdf",
                    FileName = $"IT_Helpdesk_Executive_Summary_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                IsLoading = true;

                await Task.Run(() =>
                {
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(2, Unit.Centimetre);
                            page.PageColor(Colors.White);
                            page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                            page.Header()
                                .BorderBottom(1)
                                .Padding(10)
                                .Row(row =>
                                {
                                    row.RelativeItem().Column(column =>
                                    {
                                        column.Item().Text("IT HELPDESK SYSTEM")
                                            .FontSize(20)
                                            .Bold()
                                            .FontColor(Colors.Blue.Darken3);

                                        column.Item().Text("Executive Summary & Insights Report")
                                            .FontSize(14)
                                            .FontColor(Colors.Grey.Darken2);
                                    });
                                });

                            page.Content()
                                .PaddingVertical(20)
                                .Column(column =>
                                {
                                    column.Spacing(15);

                                    // Report Information
                                    column.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(col =>
                                        {
                                            string dateRange = FromDate.HasValue && ToDate.HasValue
                                                ? $"{FromDate.Value:MMMM dd, yyyy} to {ToDate.Value:MMMM dd, yyyy}"
                                                : "All Time";

                                            col.Item().Text(text =>
                                            {
                                                text.Span("Report Period: ").Bold();
                                                text.Span(dateRange);
                                            });

                                            col.Item().Text(text =>
                                            {
                                                text.Span("Generated: ").Bold();
                                                text.Span($"{DateTime.Now:MMMM dd, yyyy hh:mm tt}");
                                            });

                                            col.Item().Text(text =>
                                            {
                                                text.Span("Total Tickets: ").Bold();
                                                text.Span(AllTickets.Count.ToString());
                                            });
                                        });
                                    });

                                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                                    // Status Summary Section
                                    column.Item().Text("Status Summary")
                                        .FontSize(16)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken2);

                                    column.Item().PaddingLeft(20).Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(1);
                                        });

                                        // Header
                                        table.Header(header =>
                                        {
                                            header.Cell().Background(Colors.Grey.Lighten3)
                                                .Padding(5).Text("Status").Bold();
                                            header.Cell().Background(Colors.Grey.Lighten3)
                                                .Padding(5).Text("Count").Bold();
                                        });

                                        // Data
                                        table.Cell().Padding(5).Text("Pending");
                                        table.Cell().Padding(5).Text(PendingCount.ToString());

                                        table.Cell().Background(Colors.Grey.Lighten4).Padding(5).Text("In Progress");
                                        table.Cell().Background(Colors.Grey.Lighten4).Padding(5).Text(InProgressCount.ToString());

                                        table.Cell().Padding(5).Text("Resolved");
                                        table.Cell().Padding(5).Text(ResolvedCount.ToString());

                                        table.Cell().Background(Colors.Grey.Lighten4).Padding(5).Text("Closed");
                                        table.Cell().Background(Colors.Grey.Lighten4).Padding(5).Text(ClosedCount.ToString());
                                    });

                                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                                    // Executive Summary Section
                                    column.Item().Text("Executive Summary & Insights")
                                        .FontSize(16)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken2);

                                    column.Item().PaddingLeft(20).Text(ReportSummary)
                                        .FontSize(12)
                                        .LineHeight(1.5f)
                                        .FontFamily("Times New Roman");

                                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                                    // Category Distribution
                                    if (CategoryStats.Count > 0)
                                    {
                                        column.Item().Text("Category Distribution")
                                            .FontSize(14)
                                            .Bold()
                                            .FontColor(Colors.Blue.Darken2);

                                        column.Item().PaddingLeft(20).Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(1);
                                            });

                                            table.Header(header =>
                                            {
                                                header.Cell().Background(Colors.Grey.Lighten3)
                                                    .Padding(5).Text("Category").Bold();
                                                header.Cell().Background(Colors.Grey.Lighten3)
                                                    .Padding(5).Text("Count").Bold();
                                            });

                                            bool alternate = false;
                                            foreach (var cat in CategoryStats)
                                            {
                                                var bgColor = alternate ? Colors.Grey.Lighten4 : Colors.White;
                                                table.Cell().Background(bgColor).Padding(5).Text(cat.Category);
                                                table.Cell().Background(bgColor).Padding(5).Text(cat.Count.ToString());
                                                alternate = !alternate;
                                            }
                                        });
                                    }
                                });

                            page.Footer()
                                .AlignCenter()
                                .DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken1))
                                .Text(text =>
                                {
                                    text.Span("Page ");
                                    text.CurrentPageNumber();
                                    text.Span(" of ");
                                    text.TotalPages();
                                    text.Span(" | IT Helpdesk System | Confidential");
                                });
                        });
                    });

                    document.GeneratePdf(saveFileDialog.FileName);
                });

                MessageBox.Show($"Executive summary exported successfully to:\n{saveFileDialog.FileName}",
                    "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}",
                    "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}
