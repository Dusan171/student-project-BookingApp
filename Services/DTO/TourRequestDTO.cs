using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Domain.Model;
using BookingApp.Services.DTO;

namespace BookingApp.Services.DTO
{
    public class TourRequestDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _touristId;
        private string _city = string.Empty;
        private string _country = string.Empty;
        private string _description = string.Empty;
        private string _language = string.Empty;
        private int _numberOfPeople;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private DateTime _createdAt;
        private TourRequestStatus _status;
        private int? _acceptedByGuideId;
        private DateTime? _acceptedDate;
        private DateTime? _scheduledDate;
        private string _statusText = string.Empty;
        private string _locationText = string.Empty;
        private string _dateRangeText = string.Empty;
        private List<TourRequestParticipantDTO> _participants = new List<TourRequestParticipantDTO>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public int TouristId
        {
            get => _touristId;
            set { _touristId = value; OnPropertyChanged(); }
        }

        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged();
                UpdateLocationText();
            }
        }

        public string Country
        {
            get => _country;
            set
            {
                _country = value;
                OnPropertyChanged();
                UpdateLocationText();
            }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string Language
        {
            get => _language;
            set { _language = value; OnPropertyChanged(); }
        }

        public int NumberOfPeople
        {
            get => _numberOfPeople;
            set { _numberOfPeople = value; OnPropertyChanged(); }
        }

        public DateTime DateFrom
        {
            get => _dateFrom;
            set
            {
                _dateFrom = value;
                OnPropertyChanged();
                UpdateDateRangeText();
            }
        }

        public DateTime DateTo
        {
            get => _dateTo;
            set
            {
                _dateTo = value;
                OnPropertyChanged();
                UpdateDateRangeText();
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); }
        }

        public TourRequestStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                StatusText = GetStatusText(value);
                OnPropertyChanged();
            }
        }

        public int? AcceptedByGuideId
        {
            get => _acceptedByGuideId;
            set { _acceptedByGuideId = value; OnPropertyChanged(); }
        }

        public DateTime? AcceptedDate
        {
            get => _acceptedDate;
            set { _acceptedDate = value; OnPropertyChanged(); }
        }

        public DateTime? ScheduledDate
        {
            get => _scheduledDate;
            set { _scheduledDate = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get => _statusText;
            private set { _statusText = value; OnPropertyChanged(); }
        }

        public string LocationText
        {
            get => _locationText;
            private set { _locationText = value; OnPropertyChanged(); }
        }

        public string DateRangeText
        {
            get => _dateRangeText;
            private set { _dateRangeText = value; OnPropertyChanged(); }
        }

        public List<TourRequestParticipantDTO> Participants
        {
            get => _participants;
            set { _participants = value; OnPropertyChanged(); }
        }

        public bool IsValid => Status != TourRequestStatus.INVALID && DateFrom > DateTime.Now.AddDays(2);
        public bool IsPending => Status == TourRequestStatus.PENDING;
        public bool IsAccepted => Status == TourRequestStatus.ACCEPTED;

        public TourRequestDTO()
        {
            CreatedAt = DateTime.Now;
            Status = TourRequestStatus.PENDING;
            Participants = new List<TourRequestParticipantDTO>();
        }

        public TourRequestDTO(TourRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Id = request.Id;
            TouristId = request.TouristId;
            City = request.City;
            Country = request.Country;
            Description = request.Description;
            Language = request.Language;
            NumberOfPeople = request.NumberOfPeople;
            DateFrom = request.DateFrom;
            DateTo = request.DateTo;
            CreatedAt = request.CreatedAt;
            Status = request.Status;
            AcceptedByGuideId = request.AcceptedByGuideId;
            AcceptedDate = request.AcceptedDate;
            ScheduledDate = request.ScheduledDate;

            Participants = request.Participants?.Select(p => new TourRequestParticipantDTO(p)).ToList()
                          ?? new List<TourRequestParticipantDTO>();

            UpdateLocationText();
            UpdateDateRangeText();
        }

        private void UpdateLocationText()
        {
            LocationText = $"{City}, {Country}";
        }

        private void UpdateDateRangeText()
        {
            DateRangeText = $"{DateFrom:dd.MM.yyyy} - {DateTo:dd.MM.yyyy}";
        }

        private string GetStatusText(TourRequestStatus status)
        {
            return status switch
            {
                TourRequestStatus.PENDING => "Na čekanju",
                TourRequestStatus.ACCEPTED => "Prihvaćen",
                TourRequestStatus.INVALID => "Nevažeći",
                _ => "Nepoznato"
            };
        }

        public TourRequest ToTourRequest()
        {
            var request = new TourRequest(Id, TouristId, City, Country, Description, Language,
                                        NumberOfPeople, DateFrom, DateTo)
            {
                CreatedAt = CreatedAt,
                Status = Status,
                AcceptedByGuideId = AcceptedByGuideId,
                AcceptedDate = AcceptedDate,
                ScheduledDate = ScheduledDate
            };

            request.Participants = Participants?.Select(p => p.ToTourRequestParticipant()).ToList()
                                  ?? new List<TourRequestParticipant>();

            return request;
        }

        public static TourRequestDTO FromDomain(TourRequest request)
        {
            return new TourRequestDTO(request);
        }
    }
}
