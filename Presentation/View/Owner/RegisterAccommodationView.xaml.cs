using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookingApp.Presentation.View.Owner
{
   
    public partial class RegisterAccommodationView : UserControl
    {
        public RegisterAccommodationView()
        {
            InitializeComponent();
            DataContext = Injector.CreateRegisterAccommodationViewModel();
        }
    }
}

