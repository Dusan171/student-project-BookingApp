using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Presentation;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class TourDetailsControlViewModel : INotifyPropertyChanged
    {
        private readonly ReservationGuestRepository _guestRepository;
        private readonly TouristAttendanceRepository _attendanceRepository;
        private readonly Tour _tour;

        public string TourName { get; private set; }
        public string TourDate { get; private set; }
        public string TourDuration { get; private set; }
        public string TotalTourists { get; private set; }
        public string Under18 { get; private set; }
        public string Between18And50 { get; private set; }
        public string Above50 { get; private set; }

        public ICommand BackCommand { get; }

        public TourDetailsControlViewModel(Tour tour)
        {
            _tour = tour;
            _guestRepository = new ReservationGuestRepository();
            _attendanceRepository = new TouristAttendanceRepository();

            LoadStatistics();

        }

        private void LoadStatistics()
        {
            TourName = _tour.Name;
            TourDate = _tour.StartTimes.FirstOrDefault()?.ToString();
            TourDuration = $"Tour Duration: {_tour.DurationHours}h";

            var attendance = _attendanceRepository.GetAll().Where(a => a.TourId == _tour.Id).ToList();
            var tourists = _guestRepository.GetAll()
                            .Where(t => attendance.Any(a => a.GuestId == t.Id))
                            .ToList();

            int under18 = tourists.Count(t => t.Age < 18);
            int between18and50 = tourists.Count(t => t.Age >= 18 && t.Age <= 50);
            int above50 = tourists.Count(t => t.Age > 50);

            int total = under18 + between18and50 + above50;
            TotalTourists = $"Total number of Tourists: {total}";
            if (total == 0) total = 1;
            Under18 = $"Under 18: {under18} ({(under18 * 100.0 / total):0.##}%)";
            Between18And50 = $"18-50: {between18and50} ({(between18and50 * 100.0 / total):0.##}%)";
            Above50 = $"Above 50: {above50} ({(above50 * 100.0 / total):0.##}%)";

            OnPropertyChanged(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
