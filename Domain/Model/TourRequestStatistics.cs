using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Model
{
    public class TourRequestStatistics
    {
        public int TotalRequests { get; set; }
        public int AcceptedCount { get; set; }
        public int NotAcceptedCount { get; set; }
        public double AcceptanceRate { get; set; }
        public double AveragePeopleInAcceptedRequests { get; set; }
    }
}
