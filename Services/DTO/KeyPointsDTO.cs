using BookingApp.Domain.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Services.DTO
{
    public class KeyPointDTO : INotifyPropertyChanged
    {
        private int _id;
        private string _name;

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
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public KeyPointDTO() { }

        public KeyPointDTO(KeyPoint kp)
        {
            Id = kp.Id;
            Name = kp.Name;
        }

        public KeyPoint ToKeyPoint()
        {
            return new KeyPoint
            {
                Id = this.Id,
                Name = this.Name
            };
        }
    }
}
