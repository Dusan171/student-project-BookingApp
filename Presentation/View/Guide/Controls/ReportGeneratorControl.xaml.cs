using System;
using System.Windows;
using System.Windows.Controls;

namespace BookingApp.Presentation.View.Guide.Controls
{
    public partial class ReportGeneratorControl : UserControl
    {
        public ReportGeneratorControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler GenerateReportClick;

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            GenerateReportClick?.Invoke(sender, e);
        }

        public DateTime? StartDate => StartDatePicker.SelectedDate;
        public DateTime? EndDate => EndDatePicker.SelectedDate;
    }
}