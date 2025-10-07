using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class AccommodationStatisticsSummaryDTO
    {
        public int TotalReservations { get; set; }
        public int TotalCancellations { get; set; }
        public int TotalReschedules { get; set; }
        public int BestYear { get; set; }
        public string BestPeriodDescription { get; set; }
        public double BestPeriodOccupancy { get; set; }

        public AccommodationStatisticsSummaryDTO()
        {
            BestPeriodDescription = "No data available";
        }
    }
}
