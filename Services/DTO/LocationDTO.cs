using BookingApp.Domain;
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
        private string _city;
        private string _country;

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

        public string City
        {
            get => _city;
            set
            {
                if (_city != value)
                {
                    _city = value;
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
                    _country = value;
                    OnPropertyChanged();
                }
            }
        }

       
        public LocationDTO() { }

        public LocationDTO(Location l)
        {
            _id =l.Id;
            _city =l.City;
            _country =l.Country;
        }

       
        public Location ToLocation()
        {
            return new Location(Id, City, Country);
        }
    }
}

