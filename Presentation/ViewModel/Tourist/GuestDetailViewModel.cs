using System.ComponentModel;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class GuestDetailViewModel : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private int _age;
        private string _email;
        private bool _isMainContact;

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(nameof(FirstName)); }
        }

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(nameof(LastName)); }
        }

        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(nameof(Age)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

       
        public bool IsMainContact
        {
            get => _isMainContact;
            set { _isMainContact = value; OnPropertyChanged(nameof(IsMainContact)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
