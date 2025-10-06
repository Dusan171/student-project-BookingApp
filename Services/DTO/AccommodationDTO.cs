using BookingApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services.DTO
{
    public class AccommodationDTO : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private LocationDTO _geoLocation;
        private string _city;
        private string _country;
        private string _type;
        private int? _maxGuests;
        private int? _minReservationDays;
        private int _cancellationDeadlineDays;
        private List<AccommodationImageDTO> _imagePaths;
        private int _ownerId; 

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value; OnPropertyChanged();
                }
            }
        }

        public LocationDTO GeoLocation
        {
            get => _geoLocation;
            set
            {
                if (_geoLocation != value)
                {
                    _geoLocation = value; OnPropertyChanged();
                }
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value; OnPropertyChanged();
                }
            }
        }

        public int? MaxGuests
        {
            get => _maxGuests;
            set
            {
                if (_maxGuests != value)
                {
                    _maxGuests = value; OnPropertyChanged();
                }
            }
        }

        public int? MinReservationDays
        {
            get => _minReservationDays;
            set
            {
                if (_minReservationDays != value)
                {
                    _minReservationDays = value; OnPropertyChanged();
                }
            }
        }

        public int CancellationDeadlineDays
        {
            get => _cancellationDeadlineDays;
            set
            {
                if (_cancellationDeadlineDays != value)
                {
                    _cancellationDeadlineDays = value; OnPropertyChanged();
                }
            }
        }

        public List<AccommodationImageDTO> ImagePaths
        {
            get => _imagePaths;
            set
            {
                if (_imagePaths != value)
                {
                    _imagePaths = value; OnPropertyChanged();
                }
            }
        }

        
        public int OwnerId
        {
            get => _ownerId;
            set
            {
                if (_ownerId != value)
                {
                    _ownerId = value; OnPropertyChanged();
                }
            }
        }

        public AccommodationDTO()
        {
            GeoLocation = new LocationDTO();
            _imagePaths = new List<AccommodationImageDTO>();
            _ownerId = 1; 
        }

        public AccommodationDTO(Accommodation a)
        {
            Id = a.Id;
            Name = a.Name;
            GeoLocation = new LocationDTO(a.GeoLocation);
            Type = a.Type.ToString();
            MaxGuests = a.MaxGuests;
            MinReservationDays = a.MinReservationDays;
            CancellationDeadlineDays = a.CancellationDeadlineDays;
            ImagePaths = a.Images.Select(i => new AccommodationImageDTO { Path = i.Path }).ToList();
            OwnerId = a.OwnerId; 
        }

        public Accommodation ToAccommodation()
        {
            var accommodation = new Accommodation
            {
                Id = this.Id,
                Name = this.Name,
                GeoLocation = this.GeoLocation.ToLocation(),
                Type = Enum.Parse<AccommodationType>(this.Type),
                MaxGuests = this.MaxGuests,
                MinReservationDays = this.MinReservationDays,
                CancellationDeadlineDays = this.CancellationDeadlineDays,
                Images = this.ImagePaths.Select(i => new AccommodationImage { Path = i.Path }).ToList(),
                OwnerId = this.OwnerId 
            };
            return accommodation;
        }
    }
}