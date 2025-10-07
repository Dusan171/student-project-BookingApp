using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class NotifiedTourDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Language { get; set; }
        public DateTime Date { get; set; }
        public string GuideName { get; set; }
        public int MaxGuests { get; set; }
        public string Description { get; set; }
    }
}
