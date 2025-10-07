using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Presentation.ViewModel.Tourist;

namespace BookingApp.Services.DTO
{
    public class TourNotificationDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TourId { get; set; }
        public NotifiedTourDTO Tour { get; set; }
    }
}
