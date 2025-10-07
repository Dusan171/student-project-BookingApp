using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Utilities;
using MvvmHelpers;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class GuestItemViewModel : BaseViewModel
    {
        public int GuestId { get; set; }
        public string FullName { get; set; }

        private bool _hasAppeared;
        public bool HasAppeared
        {
            get => _hasAppeared;
            set { _hasAppeared = value; OnPropertyChanged(); }
        }

        private int _selectedKeyPointId = -1;
        public int SelectedKeyPointId
        {
            get => _selectedKeyPointId;
            set { _selectedKeyPointId = value; OnPropertyChanged(); }
        }

        public List<KeyPoint> AvailableKeyPoints { get; set; } = new List<KeyPoint>();
    }

    public class ReservedTouristsViewModel : BaseViewModel
    {
        private readonly ReservationGuestRepository guestRepository = new ReservationGuestRepository();

        public ObservableCollection<GuestItemViewModel> GuestViewModels { get; set; } = new ObservableCollection<GuestItemViewModel>();

        private List<TouristAttendance> _attendance = new List<TouristAttendance>();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<bool>? PageClosed;

        public ReservedTouristsViewModel()
        {
            SaveCommand = new RelayCommand(o => Save());
            CancelCommand = new RelayCommand(o => Cancel());
        }

        public void Initialize(Tour tour, List<KeyPoint> passedKeyPoints, List<ReservationGuest> guests, List<TouristAttendance> attendance)
        {
            _attendance = attendance;

            GuestViewModels.Clear();

            foreach (var guest in guests)
            {
                var fullGuest = guestRepository.GetById(guest.Id);
                var att = attendance.FirstOrDefault(a => a.GuestId == guest.Id);
                if (att == null)
                {
                    att = new TouristAttendance { GuestId = guest.Id, HasAppeared = false, KeyPointJoinedAt = -1 };
                    _attendance.Add(att);
                }

                GuestViewModels.Add(new GuestItemViewModel
                {
                    GuestId = guest.Id,
                    FullName = fullGuest.FirstName + " " + fullGuest.LastName,
                    HasAppeared = att.HasAppeared,
                    SelectedKeyPointId = att.KeyPointJoinedAt,
                    AvailableKeyPoints = passedKeyPoints
                });
            }
        }

        private void Save()
        {
            foreach (var guestVm in GuestViewModels)
            {
                var att = _attendance.First(a => a.GuestId == guestVm.GuestId);
                att.HasAppeared = guestVm.HasAppeared;
                att.KeyPointJoinedAt = guestVm.HasAppeared ? guestVm.SelectedKeyPointId : -1;
            }

            PageClosed?.Invoke(this, true);
        }

        private void Cancel()
        {
            PageClosed?.Invoke(this, false);
        }

        public List<TouristAttendance> GetUpdatedAttendance() => _attendance;
    }
}
