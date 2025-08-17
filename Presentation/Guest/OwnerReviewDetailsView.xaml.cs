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
using BookingApp.Domain;

namespace BookingApp.Presentation.Guest
{
    /// <summary>
    /// Interaction logic for OwnerReviewDetailsView.xaml
    /// </summary>
    public partial class OwnerReviewDetailsView : Window
    {
        public OwnerReviewDetailsView(GuestReview review)
        {
            InitializeComponent();
            this.DataContext = review;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
