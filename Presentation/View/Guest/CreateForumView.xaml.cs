using System.Windows;
using BookingApp.Presentation.ViewModel.Guest;

namespace BookingApp.Presentation.View.Guest
{
    public partial class CreateForumView : Window
    {
        public CreateForumView()
        {
            InitializeComponent();
            var viewModel = new CreateForumViewModel();
            DataContext = viewModel;
            viewModel.CloseAction = this.Close;
        }
    }
}