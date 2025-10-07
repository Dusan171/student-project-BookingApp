using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BookingApp.Domain.Interfaces;
using BookingApp.Services.DTO;
using BookingApp.Utilities;

namespace BookingApp.Presentation.ViewModel.Tourist
{
    public class TourPartsDetailViewModel : INotifyPropertyChanged
    {
        private ComplexTourRequestDTO _selectedRequest;
        private ObservableCollection<ComplexTourRequestPartDTO> _tourParts;
        private bool _isLoading;
        private readonly int _currentUserId;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action BackRequested;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TourPartsDetailViewModel(ComplexTourRequestDTO selectedRequest, int currentUserId)
        {
            _currentUserId = currentUserId;
            _tourParts = new ObservableCollection<ComplexTourRequestPartDTO>();

            BackToListCommand = new RelayCommand(BackToList);
            RefreshCommand = new RelayCommand(RefreshData);

            SelectedRequest = selectedRequest;
        }

        public ComplexTourRequestDTO SelectedRequest
        {
            get => _selectedRequest;
            set
            {
                _selectedRequest = value;
                OnPropertyChanged();
                LoadTourParts();
            }
        }

        public ObservableCollection<ComplexTourRequestPartDTO> TourParts
        {
            get => _tourParts;
            set
            {
                _tourParts = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackToListCommand { get; }
        public ICommand RefreshCommand { get; }

        private void BackToList()
        {
            BackRequested?.Invoke();
        }

        private void RefreshData()
        {
            LoadTourParts();
        }

        private void LoadTourParts()
        {
            if (SelectedRequest == null) return;

            try
            {
                IsLoading = true;
                TourParts.Clear();

                
                var complexService = Services.Injector.CreateInstance<IComplexTourRequestService>();
                var parts = complexService.GetPartsByRequest(SelectedRequest.Id);

                foreach (var part in parts)
                {
                    TourParts.Add(part);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Greška pri učitavanju delova ture: {ex.Message}",
                                             "Greška", System.Windows.MessageBoxButton.OK,
                                             System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

       
    }
}