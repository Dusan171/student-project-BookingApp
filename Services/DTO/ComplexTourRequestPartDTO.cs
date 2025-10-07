using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookingApp.Domain.Model;

namespace BookingApp.Services.DTO
{
    public class ComplexTourRequestPartDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _complexTourRequestId;
        private int _touristId;
        private int _partIndex;
        private string _city = string.Empty;
        private string _country = string.Empty;
        private string _description = string.Empty;
        private string _language = string.Empty;
        private int _numberOfPeople;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private TourRequestStatus _status;
        private int? _acceptedByGuideId;
        private DateTime? _acceptedDate;
        private DateTime? _scheduledDate;
        private string _statusText = string.Empty;
        private string _locationText = string.Empty;
        private string _dateRangeText = string.Empty;
        private List<ComplexTourRequestParticipantDTO> _participants = new List<ComplexTourRequestParticipantDTO>();

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

        public int ComplexTourRequestId
        {
            get => _complexTourRequestId;
            set { _complexTourRequestId = value; OnPropertyChanged(); }
        }

        public int TouristId
        {
            get => _touristId;
            set { _touristId = value; OnPropertyChanged(); }
        }

        public int PartIndex
        {
            get => _partIndex;
            set { _partIndex = value; OnPropertyChanged(); }
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

        public List<ComplexTourRequestParticipantDTO> Participants
        {
            get => _participants;
            set { _participants = value; OnPropertyChanged(); }
        }

        public bool IsAccepted => Status == TourRequestStatus.ACCEPTED && AcceptedByGuideId.HasValue;
        public bool IsPending => Status == TourRequestStatus.PENDING;

        public ComplexTourRequestPartDTO()
        {
            Status = TourRequestStatus.PENDING;
            DateFrom = DateTime.Now.AddDays(3);
            DateTo = DateTime.Now.AddDays(5);
            Participants = new List<ComplexTourRequestParticipantDTO>();
        }

        public ComplexTourRequestPartDTO(ComplexTourRequestPart part)
        {
            if (part == null)
                throw new ArgumentNullException(nameof(part));

            Id = part.Id;
            ComplexTourRequestId = part.ComplexTourRequestId;
            TouristId = part.TouristId;
            PartIndex = part.PartIndex;
            City = part.City;
            Country = part.Country;
            Description = part.Description;
            Language = part.Language;
            NumberOfPeople = part.NumberOfPeople;
            DateFrom = part.DateFrom;
            DateTo = part.DateTo;
            Status = part.Status;
            AcceptedByGuideId = part.AcceptedByGuideId;
            AcceptedDate = part.AcceptedDate;
            ScheduledDate = part.ScheduledDate;
            Participants = part.Participants?.Select(p => new ComplexTourRequestParticipantDTO(p)).ToList()
                          ?? new List<ComplexTourRequestParticipantDTO>();

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

        public ComplexTourRequestPart ToComplexTourRequestPart()
        {
            /*var part = new ComplexTourRequestPart(Id, ComplexTourRequestId, TouristId, PartIndex,
                                                City, Country, Description, Language,
                                                NumberOfPeople, DateFrom, DateTo)
            {
                Status = Status,
                AcceptedByGuideId = AcceptedByGuideId,
                AcceptedDate = AcceptedDate,
                ScheduledDate = ScheduledDate
            };

            part.Participants = Participants?.Select(p => p.ToComplexTourRequestParticipant()).ToList()
                              ?? new List<ComplexTourRequestParticipant>();

            return part;*/
            return null;
        }

        public static ComplexTourRequestPartDTO FromDomain(ComplexTourRequestPart part)
        {
            return new ComplexTourRequestPartDTO(part);
        }
    }
}