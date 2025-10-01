using System;
using System.Collections.Generic;
using System.Windows;

using BookingApp.Presentation.ViewModel.Guest;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Guest
{
    public partial class SuggestedDatesView : Window
    {
        public SuggestedDatesView(List<DateRange> suggestions)
        {
            InitializeComponent();
            var viewModel = new SuggestedDatesViewModel(suggestions);
            DataContext = viewModel;
            viewModel.CloseAction = this.Close;
        }
    }
}
