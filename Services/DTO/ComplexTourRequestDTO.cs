using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BookingApp.Domain.Model;

namespace BookingApp.Services.DTO
{
    public class ComplexTourRequestDTO : INotifyPropertyChanged
    {
        private int _id;
        private int _touristId;
        private DateTime _createdAt;
        private ComplexTourRequestStatus _status;
        private DateTime _invalidationDeadline;
        private string _statusText = string.Empty;
        private string _locationText = string.Empty;
        private string _dateRangeText = string.Empty;
        private List<ComplexTourRequestPartDTO> _parts = new List<ComplexTourRequestPartDTO>();

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

        public DateTime CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); }
        }

        public ComplexTourRequestStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                StatusText = GetStatusText(value);
                OnPropertyChanged();
            }
        }

        public DateTime InvalidationDeadline
        {
            get => _invalidationDeadline;
            set { _invalidationDeadline = value; OnPropertyChanged(); }
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

        public List<ComplexTourRequestPartDTO> Parts
        {
            get => _parts;
            set
            {
                _parts = value;
                OnPropertyChanged();
                UpdateDisplayTexts();
            }
        }

        public int TotalParts => Parts.Count;
        public int AcceptedParts => Parts.Count(p => p.Status == TourRequestStatus.ACCEPTED);
        public bool IsValid => Status != ComplexTourRequestStatus.INVALID && DateTime.Now <= InvalidationDeadline;
        public bool IsPending => Status == ComplexTourRequestStatus.PENDING;
        public bool IsAccepted => Status == ComplexTourRequestStatus.ACCEPTED;

        public ComplexTourRequestDTO()
        {
            CreatedAt = DateTime.Now;
            Status = ComplexTourRequestStatus.PENDING;
            Parts = new List<ComplexTourRequestPartDTO>();
        }

        public ComplexTourRequestDTO(ComplexTourRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Id = request.Id;
            TouristId = request.TouristId;
            CreatedAt = request.CreatedAt;
            Status = request.Status;
            InvalidationDeadline = request.InvalidationDeadline;
            Parts = request.Parts?.Select(p => new ComplexTourRequestPartDTO(p)).ToList()
                   ?? new List<ComplexTourRequestPartDTO>();

            UpdateDisplayTexts();
        }

        private void UpdateDisplayTexts()
        {
            if (Parts.Any())
            {
                var firstPart = Parts.OrderBy(p => p.PartIndex).First();
                LocationText = $"{firstPart.City}, {firstPart.Country} + {Parts.Count - 1} više";

                var earliestDate = Parts.Min(p => p.DateFrom);
                var latestDate = Parts.Max(p => p.DateTo);
                DateRangeText = $"{earliestDate:dd.MM.yyyy} - {latestDate:dd.MM.yyyy}";
            }
            else
            {
                LocationText = "Nema delova";
                DateRangeText = "";
            }
        }

        private string GetStatusText(ComplexTourRequestStatus status)
        {
            return status switch
            {
                ComplexTourRequestStatus.PENDING => "Na čekanju",
                ComplexTourRequestStatus.ACCEPTED => "Prihvaćen",
                ComplexTourRequestStatus.INVALID => "Nevažeći",
                _ => "Nepoznato"
            };
        }

        public ComplexTourRequest ToComplexTourRequest()
        {
            var request = new ComplexTourRequest(Id, TouristId)
            {
                CreatedAt = CreatedAt,
                Status = Status,
                InvalidationDeadline = InvalidationDeadline
            };

            request.Parts = Parts?.Select(p => p.ToComplexTourRequestPart()).ToList()
                          ?? new List<ComplexTourRequestPart>();

            return request;
        }

        public static ComplexTourRequestDTO FromDomain(ComplexTourRequest request)
        {
            return new ComplexTourRequestDTO(request);
        }
    }
}