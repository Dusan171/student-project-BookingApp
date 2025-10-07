using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Model
{
    public class LocationStatisticData
    {
        public string Location { get; set; }
        public int Count { get; set; }
        public double BarWidth { get; set; }
    }
}
