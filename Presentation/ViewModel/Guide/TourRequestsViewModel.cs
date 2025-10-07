using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookingApp.Domain.Model;
using BookingApp.Utilities;
using System.Windows;
using BookingApp.Repositories;
using System.Collections.Generic;
using MvvmHelpers.Commands;

namespace BookingApp.Presentation.View.Guide
{
    public class ComplexTourRequestViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;

        public ComplexTourRequestViewModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public int Id { get; set; }
        public ObservableCollection<ComplexTourRequestPart> Parts { get; set; }

        public DateTime? DateFrom => Parts?.OrderBy(p => p.DateFrom).FirstOrDefault()?.DateFrom;
        public DateTime? DateTo => Parts?.OrderByDescending(p => p.DateTo).FirstOrDefault()?.DateTo;

        public int? TouristId => Parts?.FirstOrDefault()?.TouristId;

        
        public string TouristName
        {
            get
            {
                if (TouristId == null) return string.Empty;
                var user = _userRepository.GetById(TouristId.Value);
                return (user?.FirstName + " " + user?.LastName) ?? $"Unknown Tourist {TouristId}";
            }
        }
    }


    
    public class TourRequestsViewModel : BaseViewModel
    {
        private string _city;
        public string City { get => _city; set => SetProperty(ref _city, value); }

        private string _country;
        public string Country { get => _country; set => SetProperty(ref _country, value); }

        private string _language;
        public string Language { get => _language; set => SetProperty(ref _language, value); }

        private int? _numberOfPeople;
        public int? NumberOfPeople { get => _numberOfPeople; set => SetProperty(ref _numberOfPeople, value); }

        private DateTime? _dateFrom;
        public DateTime? DateFrom { get => _dateFrom; set => SetProperty(ref _dateFrom, value); }

        private DateTime? _dateTo;
        public DateTime? DateTo { get => _dateTo; set => SetProperty(ref _dateTo, value); }

        public ObservableCollection<TourRequest> Requests { get; set; }
        public ObservableCollection<ComplexTourRequestViewModel> ComplexRequests { get; set; }
        public Action<TourRequest>? NavigateToTourDetails { get; set; }
        public Action<ComplexTourRequestViewModel>? NavigateToComplexParts { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand FilterSimpleCommand { get; }
        public ICommand FilterComplexCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand ViewComplexPartsCommand { get; }

        private readonly UserRepository _userRepository;

        private readonly ObservableCollection<TourRequest> _allRequests;
        private readonly ObservableCollection<ComplexTourRequestPart> _allComplexParts;



        public TourRequestsViewModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
            ViewDetailsCommand = new Command<int>(ViewDetails);
            ViewComplexPartsCommand = new Command<int>(ViewComplexParts);
            Requests = new ObservableCollection<TourRequest>();
            ComplexRequests = new ObservableCollection<ComplexTourRequestViewModel>();
            TourRequestRepository repo = new TourRequestRepository();
            ComplexTourRequestPartRepository complexPartRepo = new ComplexTourRequestPartRepository();
            _allRequests = new ObservableCollection<TourRequest>(repo.GetAll());

            _allComplexParts = new ObservableCollection<ComplexTourRequestPart>(complexPartRepo.GetAll());




            LoadCommand = new RelayCommand(LoadRequests);
            FilterSimpleCommand = new RelayCommand(FilterSimpleRequests);
            FilterComplexCommand = new RelayCommand(FilterComplexRequests);
            ViewDetailsCommand = new RelayCommand<int>(ViewDetails);
            ViewComplexPartsCommand = new RelayCommand<int>(ViewComplexParts);

            LoadRequests();
        }

        private void LoadRequests()
        {
            Requests.Clear();
            foreach (var r in _allRequests) Requests.Add(r);

            ComplexRequests.Clear();
            var grouped = _allComplexParts.GroupBy(p => p.ComplexTourRequestId)
                .Select(g => new ComplexTourRequestViewModel(_userRepository)
                {
                    Id = g.Key,
                    Parts = new ObservableCollection<ComplexTourRequestPart>(g)
                });

            foreach (var cr in grouped) ComplexRequests.Add(cr);
        }

        private void FilterSimpleRequests()
        {
            var filtered = _allRequests.Where(r =>
                (string.IsNullOrEmpty(City) || r.City.Contains(City, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(Country) || r.Country.Contains(Country, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(Language) || r.Language.Contains(Language, StringComparison.OrdinalIgnoreCase)) &&
                (!NumberOfPeople.HasValue || r.NumberOfPeople == NumberOfPeople.Value) &&
                (!DateFrom.HasValue || r.DateFrom >= DateFrom.Value) &&
                (!DateTo.HasValue || r.DateTo <= DateTo.Value)
            );

            Requests.Clear();
            foreach (var r in filtered) Requests.Add(r);
        }

        private void FilterComplexRequests()
        {
            var filtered = _allComplexParts
                .Where(p =>
                    (string.IsNullOrEmpty(City) || p.City.Contains(City, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(Country) || p.Country.Contains(Country, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(Language) || p.Language.Contains(Language, StringComparison.OrdinalIgnoreCase)) &&
                    (!NumberOfPeople.HasValue || p.NumberOfPeople == NumberOfPeople.Value) &&
                    (!DateFrom.HasValue || p.DateFrom >= DateFrom.Value) &&
                    (!DateTo.HasValue || p.DateTo <= DateTo.Value)
                )
                .GroupBy(p => p.ComplexTourRequestId)
                .Select(g => new ComplexTourRequestViewModel(_userRepository)
                {
                    Id = g.Key,
                    Parts = new ObservableCollection<ComplexTourRequestPart>(g)
                });

            ComplexRequests.Clear();
            foreach (var cr in filtered) ComplexRequests.Add(cr);
        }

        private void ViewDetails(int requestId)
        {
            var request = Requests.FirstOrDefault(c => c.Id == requestId);
            if (request != null)
            {
                NavigateToTourDetails?.Invoke(request);
            }
        }

        private void ViewComplexParts(int complexRequestId)
        {
            var complexRequest = ComplexRequests.FirstOrDefault(c => c.Id == complexRequestId);
            if (complexRequest != null)
            {
                NavigateToComplexParts?.Invoke(complexRequest);
            }
        }
    }
}
