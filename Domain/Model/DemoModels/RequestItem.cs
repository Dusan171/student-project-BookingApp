using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Domain.Model.DemoModels
{
    public class RequestItem
    {
        public string Guest { get; set; }
        public string Accommodation { get; set; }
        public string OriginalDates { get; set; }
        public string RequestedDates { get; set; }
        public bool IsAvailable { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
    }

}
