using BookingApp.Domain.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class AccommodationImageDTO : INotifyPropertyChanged
    { 
        private int _id;
        private string _path;
        private int _accommodationId;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }  
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value; OnPropertyChanged();
                }
            }
        }
        public string Path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value; OnPropertyChanged();
                }
            }
        }
        public int AccommodationId
        {
            get => _accommodationId;
            set
            {
                if (_accommodationId != value)
                {
                    _accommodationId = value; OnPropertyChanged();
                }
            }
        }
        public AccommodationImageDTO() { }
        public AccommodationImageDTO(AccommodationImage a)
        {
            _id = a.Id;
            _path =a.Path;
            _accommodationId = a.AccommodationId;
        }
        public AccommodationImageDTO FromModel(AccommodationImage model)
        {
            return new AccommodationImageDTO
            {
                Id = model.Id,
                Path = model.Path,
                AccommodationId = model.AccommodationId
            };
        }
        public AccommodationImage ToAccommodationImage()
        {
            return new AccommodationImage
            {
                Id = this.Id,
                Path = this.Path,
                AccommodationId = this.AccommodationId
            };
        }
    }
}
