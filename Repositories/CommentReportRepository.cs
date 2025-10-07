using BookingApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BookingApp.Repositories
{
    public class CommentReportRepository : ICommentReportRepository
    {
        private const string FilePath = "../../../Resources/Data/commentReports.csv";
        private List<(int CommentId, int ReporterId)> _reports;

        public CommentReportRepository()
        {
            _reports = LoadFromCSV();
        }

        private List<(int CommentId, int ReporterId)> LoadFromCSV()
        {
            var reports = new List<(int, int)>();

            if (!File.Exists(FilePath))
            {
                File.Create(FilePath).Close();
                return reports;
            }

            var lines = File.ReadAllLines(FilePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split('|');
                if (values.Length >= 2)
                {
                    int commentId = int.Parse(values[0]);
                    int reporterId = int.Parse(values[1]);
                    reports.Add((commentId, reporterId));
                }
            }

            return reports;
        }

        private void SaveToCSV()
        {
            var lines = _reports.Select(r => $"{r.CommentId}|{r.ReporterId}");
            File.WriteAllLines(FilePath, lines);
        }

        public void ReportComment(int commentId, int reporterId)
        {
            if (!HasUserReported(commentId, reporterId))
            {
                _reports.Add((commentId, reporterId));
                SaveToCSV();
            }
        }

        public List<(int CommentId, int ReporterId)> GetAll()
        {
            return _reports.ToList();
        }

        public bool HasUserReported(int commentId, int reporterId)
        {
            return _reports.Any(r => r.CommentId == commentId && r.ReporterId == reporterId);
        }

        public int GetReportsCount(int commentId)
        {
            return _reports.Count(r => r.CommentId == commentId);
        }
    }
}