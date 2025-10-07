using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MvvmHelpers;
using MvvmHelpers.Commands;
using BookingApp.Domain.Model;
using BookingApp.Presentation.View.Guide;
using System;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class ComplexTourPartsViewModel : BaseViewModel
    {
        public string Title { get; }
        public string DateRange { get; }
        public ObservableCollection<ComplexTourRequestPart> Parts { get; }
        public Action<ComplexTourRequestPart>? NavigateToPartDetails { get; set; }

        public ICommand ViewPartDetailsCommand { get; }

        public ComplexTourPartsViewModel(ComplexTourRequestViewModel tour)
        {
            Title = $"Complex Tour #{tour.Id}";
            DateRange = $"{tour.DateFrom:dd/MM/yyyy} - {tour.DateTo:dd/MM/yyyy}";
            Parts = tour.Parts;

            ViewPartDetailsCommand = new Command<ComplexTourRequestPart>(OnViewPartDetails);
        }

        private void OnViewPartDetails(ComplexTourRequestPart part)
        {
            NavigateToPartDetails?.Invoke(part);
        }
    }
}
