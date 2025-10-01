using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Guest
{
    public class SuggestedDatesViewModel : ViewModelBase
    {
        public ObservableCollection<DateRange> SuggestedDateRanges { get; }
        public DateRange SelectedDateRange { get; set; }

        public bool IsConfirmed { get; private set; } = false;
        public Action CloseAction { get; set; }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public SuggestedDatesViewModel(List<DateRange> suggestions)
        {
            SuggestedDateRanges = new ObservableCollection<DateRange>(suggestions);

            ConfirmCommand = new RelayCommand(Confirm, CanConfirm);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanConfirm(object obj)
        {
            return SelectedDateRange != null;
        }

        private void Confirm(object obj)
        {
            IsConfirmed = true;
            CloseAction?.Invoke();
        }

        private void Cancel(object obj)
        {
            IsConfirmed = false;
            CloseAction?.Invoke();
        }
    }
}