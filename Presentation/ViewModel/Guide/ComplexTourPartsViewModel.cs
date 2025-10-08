using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MvvmHelpers;
using MvvmHelpers.Commands;
using BookingApp.Domain.Model;
using BookingApp.Presentation.View.Guide;
using BookingApp.Repositories;
using System;

namespace BookingApp.Presentation.ViewModel.Guide
{
    public class ComplexTourPartsViewModel : BaseViewModel
    {
        private ObservableCollection<ComplexTourRequestPart> _parts;
        
        public string Title { get; }
        public string DateRange { get; }
        public ObservableCollection<ComplexTourRequestPart> Parts 
        { 
            get => _parts;
            private set => SetProperty(ref _parts, value);
        }
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

        public void UpdatePart(ComplexTourRequestPart updatedPart)
        {
            var index = -1;
            for (int i = 0; i < Parts.Count; i++)
            {
                if (Parts[i].Id == updatedPart.Id)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                Parts[index] = updatedPart;
                
                var tempCollection = new ObservableCollection<ComplexTourRequestPart>(Parts);
                Parts = tempCollection;
            }
        }

        public void RefreshParts()
        {
            if (Parts?.Count > 0)
            {
                var partRepository = new ComplexTourRequestPartRepository();
                var participantRepository = new ComplexTourRequestParticipantRepository();

                for (int i = 0; i < Parts.Count; i++)
                {
                    var refreshedPart = partRepository.GetById(Parts[i].Id);
                    if (refreshedPart != null)
                    {
                        refreshedPart.Participants = participantRepository.GetByPartId(refreshedPart.Id);
                        Parts[i] = refreshedPart;
                    }
                }

                OnPropertyChanged(nameof(Parts));
            }
        }
    }
}
