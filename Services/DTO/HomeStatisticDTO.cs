using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class HomeStatisticDTO
    {
        public int TotalReviews { get; set; }
        public double AverageGrade { get; set; }
        public int ActiveProperties { get; set; }
        public string WelcomeMessage { get; set; }
    }
}
