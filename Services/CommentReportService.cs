using BookingApp.Domain.Interfaces;

namespace BookingApp.Services
{
    public class CommentReportService : ICommentReportService
    {
        private readonly ICommentReportRepository _reportRepository;

        public CommentReportService(ICommentReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public bool ReportComment(int commentId, int reporterId)
        {
            if (HasUserReported(commentId, reporterId))
                return false;

            _reportRepository.ReportComment(commentId, reporterId);
            return true;
        }

        public int GetReportsCount(int commentId)
        {
            return _reportRepository.GetReportsCount(commentId);
        }

        public bool HasUserReported(int commentId, int userId)
        {
            return _reportRepository.HasUserReported(commentId, userId);
        }
    }
}