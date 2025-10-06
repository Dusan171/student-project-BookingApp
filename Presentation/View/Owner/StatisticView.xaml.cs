using System.Windows.Controls;
using System.Windows.Input;
using BookingApp.Presentation.ViewModel.Owner;

namespace BookingApp.Presentation.View.Owner
{
    /// <summary>
    /// Interaction logic for StatisticView.xaml
    /// </summary>
    public partial class StatisticView : UserControl
    {
        public StatisticView()
        {
            InitializeComponent();
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is StatisticViewModel viewModel && viewModel.SelectedAccommodation != null)
                {
                    viewModel.LoadStatisticsCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                DataGrid_KeyDown(sender, e);
            }
        }
    }
}