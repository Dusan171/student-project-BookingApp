using BookingApp.Utilities;
using BookingApp.Presentation.Owner;
using BookingApp.Presentation.View.Guest;
using BookingApp.View.Guide;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BookingApp.Domain;
using BookingApp.Repositories;

namespace BookingApp.View
{
    /// <summary>
    /// Interaction logic for SignInForm.xaml
    /// </summary>
    public partial class SignInForm : Window
    {

        private readonly UserRepository _repository;

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (value != _username)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SignInForm()
        {
            InitializeComponent();
            DataContext = this;
            _repository = new UserRepository();
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            User user = _repository.GetByUsername(Username);

            if (user != null)
            {
                if(user.Password == txtPassword.Password)
                {
                    Session.CurrentUser = user;
                    switch (user.Role)
                    {
                        case UserRole.OWNER:
                            var ownerView = new OwnerDashboard();
                            ownerView.Show();

                            break;

                        case UserRole.GUEST:
                            var guestView = new GuestMainView();//samo ovdje treba pozvati drugi prozor
                            guestView.Show();
                            break;
                        case UserRole.GUIDE:
                            var guideView = new CreateTourForm();
                            guideView.Show();
                            break;

                        case UserRole.TOURIST:
                            var touristView = new TourSearch();
                            touristView.Show();
                            break;

                        default:
                            MessageBox.Show($"User role {user.Role} not implemented.");
                            break;
                    }

                    this.Close();
                    /*
                    CommentsOverview commentsOverview = new CommentsOverview(user);
                    commentsOverview.Show();
                    Close();
                    */
                } 
                else
                {
                    MessageBox.Show("Wrong password!");
                }
            }
            else
            {
                MessageBox.Show("Wrong username!");
            }
            
        }
    }
}
