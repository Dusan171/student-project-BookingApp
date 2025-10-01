using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class HighDemandLocationDTO
    {
        public string City { get; set; }
        public string Country { get; set; }
        public int ReservationCount { get; set; }
        public double OccupancyRate { get; set; }
        public string Recommendation { get; set; }
    }
}
