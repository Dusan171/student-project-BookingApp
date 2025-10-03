namespace BookingApp.Domain.Interfaces
{
    public interface ICommentReportService
    {
        bool ReportComment(int commentId, int reporterId);
        int GetReportsCount(int commentId);
        bool HasUserReported(int commentId, int userId);
    }
}