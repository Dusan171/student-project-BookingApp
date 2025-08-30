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
    public class LocationDTO : INotifyPropertyChanged
    {
        private int _id;
        private string _city = string.Empty;
        private string _country = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string City
        {
            get => _city;
            set
            {
                if (_city != value)
                {
                    _city = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public string Country
        {
            get => _country;
            set
            {
                if (_country != value)
                {
                    _country = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public LocationDTO() { }

        public LocationDTO(Location l)
        {
            _id = l.Id;
            _city = l.City ?? string.Empty;
            _country = l.Country ?? string.Empty;
        }

        public Location ToLocation()
        {
            return new Location(Id, City, Country);
        }
    }
}