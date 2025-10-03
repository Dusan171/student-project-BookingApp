using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Interfaces
{
    public interface ICommentReportRepository
    {
        void ReportComment(int commentId, int reporterId);
        List<(int CommentId, int ReporterId)> GetAll();
        bool HasUserReported(int commentId, int reporterId);
        int GetReportsCount(int commentId);
    }
}
