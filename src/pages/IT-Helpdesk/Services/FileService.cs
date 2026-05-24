using System.IO;
using System.Diagnostics;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk.Services
{
    public class FileService : IFileService
    {
        private readonly string _baseStoragePath;
        private readonly IAttachmentRepository _attachmentRepository;

        public FileService(IAttachmentRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
            _baseStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments");
            Directory.CreateDirectory(_baseStoragePath);
        }

        public async Task<string> SaveAttachmentAsync(string sourceFilePath, int ticketId)
        {
            var ticketFolder = Path.Combine(_baseStoragePath, $"Ticket_{ticketId}");
            Directory.CreateDirectory(ticketFolder);

            var fileName = Path.GetFileName(sourceFilePath);
            var destPath = Path.Combine(ticketFolder, fileName);

            // Handle duplicate filenames with timestamp suffix
            if (File.Exists(destPath))
            {
                var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                fileName = $"{nameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                destPath = Path.Combine(ticketFolder, fileName);
            }

            await Task.Run(() => File.Copy(sourceFilePath, destPath));

            // Store a relative path (relative to app base dir) to keep it short and portable
            string relativePath = Path.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, destPath);

            // Create Attachment record in database
            var attachment = new Attachment
            {
                TicketID   = ticketId,
                FilePath   = relativePath,
                FileName   = fileName,
                UploadedBy = SessionManager.CurrentUser?.UserID ?? 0,
                UploadedAt = DateTime.Now
            };

            await _attachmentRepository.InsertAsync(attachment);

            return destPath; // return full path so callers can open the file
        }

        public async Task<bool> DeleteAttachmentAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                await Task.Run(() => File.Delete(filePath));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> OpenAttachmentAsync(string filePath)
        {
            // Handle null or empty file path
            if (string.IsNullOrWhiteSpace(filePath))
            {
                System.Windows.MessageBox.Show(
                    "No file path is associated with this attachment.",
                    "Attachment Unavailable",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return false;
            }

            // Resolve relative path to absolute if needed
            string resolvedPath = Path.IsPathRooted(filePath)
                ? filePath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            // Verify the file exists before attempting to open it
            if (!File.Exists(resolvedPath))
            {
                System.Windows.MessageBox.Show(
                    "The attachment file could not be found. It may have been moved or deleted.",
                    "File Not Found",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return false;
            }

            try
            {
                await Task.Run(() => Process.Start(new ProcessStartInfo(resolvedPath) { UseShellExecute = true }));
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Unable to open the attachment: {ex.Message}",
                    "Error Opening File",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return false;
            }
        }

        public Task<List<Attachment>> GetAttachmentsByTicketAsync(int ticketId)
        {
            return _attachmentRepository.GetByTicketIdAsync(ticketId);
        }
    }
}
