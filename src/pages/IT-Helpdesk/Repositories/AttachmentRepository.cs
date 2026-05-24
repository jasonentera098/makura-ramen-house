using System.Data.OleDb;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk
{
    public class AttachmentRepository : RepositoryBase, IAttachmentRepository
    {
        public AttachmentRepository(string connectionString) : base(connectionString) { }

        private static Attachment MapAttachment(OleDbDataReader reader) => new Attachment
        {
            AttachmentID = reader.GetInt32(reader.GetOrdinal("AttachmentID")),
            TicketID     = reader.GetInt32(reader.GetOrdinal("TicketID")),
            FilePath     = reader["FilePath"]?.ToString() ?? string.Empty,
            FileName     = reader["FileName"]?.ToString() ?? string.Empty,
            UploadedBy   = reader.GetInt32(reader.GetOrdinal("UploadedBy")),
            UploadedAt   = reader["UploadedAt"] == DBNull.Value
                               ? DateTime.MinValue
                               : Convert.ToDateTime(reader["UploadedAt"])
        };

        public async Task<int> InsertAsync(Attachment attachment)
        {
            await ExecuteNonQueryAsync(
                "INSERT INTO Attachments (TicketID, FilePath, FileName, UploadedBy, UploadedAt) " +
                "VALUES (?, ?, ?, ?, ?)",
                [
                    new OleDbParameter("@TicketID",    attachment.TicketID),
                    new OleDbParameter("@FilePath",    attachment.FilePath),
                    new OleDbParameter("@FileName",    attachment.FileName),
                    new OleDbParameter("@UploadedBy",  attachment.UploadedBy),
                    new OleDbParameter("@UploadedAt",  attachment.UploadedAt)
                ]);

            return await ExecuteScalarAsync<int>("SELECT @@IDENTITY");
        }

        public Task<List<Attachment>> GetByTicketIdAsync(int ticketId) =>
            ExecuteReaderAsync(
                "SELECT AttachmentID, TicketID, FilePath, FileName, UploadedBy, UploadedAt " +
                "FROM Attachments WHERE TicketID = ?",
                MapAttachment,
                [new OleDbParameter("@TicketID", ticketId)]);

        public async Task<bool> DeleteAsync(int attachmentId)
        {
            int rows = await ExecuteNonQueryAsync(
                "DELETE FROM Attachments WHERE AttachmentID = ?",
                [new OleDbParameter("@AttachmentID", attachmentId)]);
            return rows > 0;
        }
    }
}
