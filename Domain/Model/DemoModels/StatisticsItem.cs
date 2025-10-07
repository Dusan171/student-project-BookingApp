using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Model.DemoModels
{
    public class StatisticsItem
    {
        public int Year { get; set; }
        public int Reservations { get; set; }
        public int Cancellations { get; set; }
        public int Modifications { get; set; }
        public double Occupancy { get; set; }
    }
}
