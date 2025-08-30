using BookingApp.Presentation.View.Owner;
using BookingApp.Presentation.ViewModel.Owner;
using BookingApp.Services;
using BookingApp.Services.DTO;
using BookingApp.Utilities;
using BookingApp.View;
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
using System.Windows.Shapes;

namespace BookingApp.Presentation.View.Owner
{
    public partial class OwnerDashboard : Window
    {
        public OwnerDashboard()
        {
            InitializeComponent();

            DataContext = Injector.CreateOwnerDashboardViewModel(Close);
        }

    }

}

