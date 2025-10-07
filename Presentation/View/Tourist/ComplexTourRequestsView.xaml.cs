using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class ComplexTourRequestsView : UserControl
    {
        private ComplexTourRequestViewModel _viewModel;

        public ComplexTourRequestViewModel ViewModel => DataContext as ComplexTourRequestViewModel;

        public ComplexTourRequestsView()
        {
            InitializeComponent();
        }

        public void InitializeViewModel(int currentUserId)
        {
            try
            {
                var viewModel = Injector.CreateComplexTourRequestViewModel(currentUserId);
                DataContext = viewModel;
                _viewModel = viewModel;

               
                viewModel.ShowDetailsRequested += OnShowDetailsRequested;
            }
            catch (Exception ex)
            {
                ShowErrorOverlay($"Greška pri inicijalizaciji prikaza složenih zahteva: {ex.Message}");
            }
        }

        private void OnShowDetailsRequested(object sender, ComplexTourRequestEventArgs e)
        {
            try
            {
                if (e.TourRequest is ComplexTourRequestDTO request)
                {
                   
                    var detailView = new TourPartsDetailView();
                    detailView.InitializeViewModel(request, ViewModel.CurrentUserId);

                   
                    detailView.BackToListRequested += OnBackToListRequested;

                   
                    var mainGrid = this.Content as Grid;
                    if (mainGrid != null)
                    {
                        
                        Panel.SetZIndex(detailView, 1000);
                        mainGrid.Children.Add(detailView);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorOverlay($"Greška pri prikazivanju detalja: {ex.Message}");
            }
        }

        private void OnBackToListRequested(object sender, EventArgs e)
        {
            try
            {
                if (sender is TourPartsDetailView detailView)
                {
                    
                    var mainGrid = this.Content as Grid;
                    if (mainGrid != null && mainGrid.Children.Contains(detailView))
                    {
                        mainGrid.Children.Remove(detailView);
                    }

                    
                    detailView.BackToListRequested -= OnBackToListRequested;

                   
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                ShowErrorOverlay($"Greška pri povratku na listu: {ex.Message}");
            }
        }

        public void RefreshData()
        {
            try
            {
                ViewModel?.RefreshCommand?.Execute(null);
            }
            catch (Exception ex)
            {
                ShowErrorOverlay($"Greška pri osvežavanju podataka: {ex.Message}");
            }
        }

        private void CreateRequest_Click(object sender, RoutedEventArgs e)
        {
            CreateRequestView.Visibility = Visibility.Visible;
            ClearAllErrors();
        }

        private void CancelCreateRequest_Click(object sender, RoutedEventArgs e)
        {
            CreateRequestView.Visibility = Visibility.Collapsed;
            ClearAllErrors();
        }

        private void SaveRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAllFields())
            {
                return;
            }

            if (!ValidateParts())
            {
                return;
            }

            try
            {
                SaveRequestSilent();
                ShowSuccessOverlay("Zahtev za složenu turu je uspešno kreiran!");
                CreateRequestView.Visibility = Visibility.Collapsed;
                ClearAllErrors();
            }
            catch (Exception ex)
            {
                ShowErrorOverlay($"Greška pri kreiranju zahteva: {ex.Message}");
            }
        }

        private void AddPart_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePartFields())
            {
                return;
            }

            if (!ValidateParticipants())
            {
                return;
            }

            try
            {
                
                _viewModel?.AddPartCommand?.Execute(null);
                ClearAllErrors();
            }
            catch (Exception ex)
            {
                ShowErrorOverlay($"Greška pri dodavanju dela ture: {ex.Message}");
            }
        }

        private void AddParticipant_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateParticipantFields())
            {
                return;
            }

            try
            {
                _viewModel?.AddParticipantCommand?.Execute(null);
                ClearParticipantErrors();
            }
            catch (Exception ex)
            {
                ShowErrorOverlay($"Greška pri dodavanju učesnika: {ex.Message}");
            }
        }

        private void SaveRequestSilent()
        {
            var requestDTO = new ComplexTourRequestDTO
            {
                TouristId = _viewModel.CurrentUserId,
                Parts = _viewModel.CurrentParts.ToList()
            };

            for (int i = 0; i < _viewModel.CurrentParts.Count; i++)
            {
                _viewModel.CurrentParts[i].PartIndex = i + 1;
            }

            var requestService = Injector.CreateInstance<IComplexTourRequestService>();
            var savedRequest = requestService.CreateRequest(requestDTO);

            
            _viewModel.FilteredRequests.Insert(0, savedRequest);

            
            _viewModel.CurrentParts.Clear();
            _viewModel.CurrentParticipants.Clear();
            _viewModel.InitializeNewPart();
        }

       
        private void ValidateField(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string fieldName = textBox.Name;
            string value = textBox.Text?.Trim();

            switch (fieldName)
            {
                case "CityTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, "Grad je obavezan");
                    }
                    else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                    {
                        ShowFieldError(textBox, "Grad može sadržavati samo slova");
                    }
                    else if (value.Length > 50)
                    {
                        ShowFieldError(textBox, "Naziv grada je predugačak");
                    }
                    else
                    {
                        HideFieldError(textBox);
                    }
                    break;

                case "CountryTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, "Država je obavezna");
                    }
                    else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                    {
                        ShowFieldError(textBox, "Država može sadržavati samo slova");
                    }
                    else if (value.Length > 50)
                    {
                        ShowFieldError(textBox, "Naziv države je predugačak");
                    }
                    else
                    {
                        HideFieldError(textBox);
                    }
                    break;

                case "DescriptionTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, "Opis je obavezan");
                    }
                    else if (value.Length < 10)
                    {
                        ShowFieldError(textBox, "Opis mora imati najmanje 10 karaktera");
                    }
                    else if (value.Length > 500)
                    {
                        ShowFieldError(textBox, "Opis je predugačak (maks. 500 karaktera)");
                    }
                    else
                    {
                        HideFieldError(textBox);
                    }
                    break;

                case "NumberOfPeopleTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, "Broj ljudi je obavezan");
                    }
                    else if (!int.TryParse(value, out int numberOfPeople) || numberOfPeople <= 0)
                    {
                        ShowFieldError(textBox, "Broj ljudi mora biti pozitivan broj");
                    }
                    else if (numberOfPeople > 50)
                    {
                        ShowFieldError(textBox, "Maksimalno 50 učesnika po delu");
                    }
                    else
                    {
                        HideFieldError(textBox);
                    }
                    break;
            }
        }

        private void ValidateComboField(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.Name == "LanguageComboBox")
            {
                if (comboBox.SelectedItem == null)
                {
                }
                else
                {
                }
            }
        }

        private void ValidateDateField(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is DatePicker datePicker)) return;

            string fieldName = datePicker.Name;

            switch (fieldName)
            {
                case "DateFromPicker":
                    if (!datePicker.SelectedDate.HasValue)
                    {
                        
                    }
                    else if (datePicker.SelectedDate <= DateTime.Now.AddDays(2))
                    {
                        
                    }
                    else
                    {
                        
                        if (DateToPicker.SelectedDate.HasValue)
                        {
                            ValidateDateRange();
                        }
                    }
                    break;

                case "DateToPicker":
                    if (!datePicker.SelectedDate.HasValue)
                    {
                        
                    }
                    else
                    {
                        ValidateDateRange();
                    }
                    break;
            }
        }

        private void ValidateDateRange()
        {
            if (DateFromPicker.SelectedDate.HasValue && DateToPicker.SelectedDate.HasValue)
            {
                if (DateToPicker.SelectedDate <= DateFromPicker.SelectedDate)
                {
                   
                }
            }
        }

        private void ValidateParticipantField(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string fieldName = textBox.Name;
            string value = textBox.Text?.Trim();

            switch (fieldName)
            {
                case "ParticipantFirstNameTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, "Ime je obavezno");
                    }
                    else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                    {
                        ShowFieldError(textBox, "Ime može sadržavati samo slova");
                    }
                    else
                    {
                        HideFieldError(textBox);
                    }
                    break;

                case "ParticipantLastNameTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, "Prezime je obavezno");
                    }
                    else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                    {
                        ShowFieldError(textBox, "Prezime može sadržavati samo slova");
                    }
                    else
                    {
                        HideFieldError(textBox);
                    }
                    break;

                case "ParticipantAgeTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, "Godine su obavezne");
                    }
                    else if (!int.TryParse(value, out int age) || age <= 0)
                    {
                        ShowFieldError(textBox, "Godine moraju biti pozitivan broj");
                    }
                    else if (age > 120)
                    {
                        ShowFieldError(textBox, "Maksimalno 120 godina");
                    }
                    else
                    {
                        HideFieldError(textBox);
                    }
                    break;
            }
        }

       
        private bool ValidateAllFields()
        {
            bool isValid = true;
            var errorMessages = new List<string>();

            

            return isValid;
        }

        private bool ValidatePartFields()
        {
            bool isValid = true;
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(_viewModel?.NewPart?.City))
            {
                errorMessages.Add("• Grad je obavezan");
                ShowFieldError(CityTextBox, "Grad je obavezan");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(_viewModel?.NewPart?.Country))
            {
                errorMessages.Add("• Država je obavezna");
                ShowFieldError(CountryTextBox, "Država je obavezna");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(_viewModel?.NewPart?.Description))
            {
                errorMessages.Add("• Opis je obavezan");
                ShowFieldError(DescriptionTextBox, "Opis je obavezan");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(_viewModel?.NewPart?.Language))
            {
                errorMessages.Add("• Jezik je obavezan");
                isValid = false;
            }

            if (_viewModel?.NewPart?.NumberOfPeople <= 0)
            {
                errorMessages.Add("• Broj ljudi mora biti veći od 0");
                ShowFieldError(NumberOfPeopleTextBox, "Broj ljudi mora biti veći od 0");
                isValid = false;
            }

            if (_viewModel?.NewPart?.DateFrom == default(DateTime) || _viewModel?.NewPart?.DateFrom <= DateTime.Now.AddDays(2))
            {
                errorMessages.Add("• Datum početka mora biti najmanje 3 dana unapred");
                isValid = false;
            }

            if (_viewModel?.NewPart?.DateTo == default(DateTime) || _viewModel?.NewPart?.DateTo <= _viewModel?.NewPart?.DateFrom)
            {
                errorMessages.Add("• Datum završetka mora biti nakon početka");
                isValid = false;
            }

            if (!isValid)
            {
                string combinedErrors = "Molimo ispravite sledeće greške u delu zahteva:\n\n" + string.Join("\n", errorMessages);
                ShowErrorOverlay(combinedErrors);
            }

            return isValid;
        }

        private bool ValidateParticipants()
        {
            if (_viewModel?.NewPart?.NumberOfPeople > 0 && _viewModel?.CurrentParticipants?.Count != _viewModel.NewPart.NumberOfPeople)
            {
                ShowErrorOverlay($"Morate dodati tačno {_viewModel.NewPart.NumberOfPeople} učesnika. Trenutno imate {_viewModel.CurrentParticipants.Count}.");
                return false;
            }
            return true;
        }

        private bool ValidateParts()
        {
            if (_viewModel?.CurrentParts?.Count == 0)
            {
                ShowErrorOverlay("Dodajte najmanje jedan deo složene ture.");
                return false;
            }
            return true;
        }

        private bool ValidateParticipantFields()
        {
            bool isValid = true;
            var errorMessages = new List<string>();

            if (_viewModel?.NewPart?.NumberOfPeople <= 0)
            {
                errorMessages.Add("• Prvo morate uneti broj ljudi za ovaj deo ture");
                ShowFieldError(ParticipantFirstNameTextBox, "Prvo morate uneti broj ljudi za ovaj deo ture");
                isValid = false;
            }
            else if (_viewModel?.CurrentParticipants?.Count >= _viewModel.NewPart.NumberOfPeople)
            {
                errorMessages.Add($"• Već ste dodali maksimalan broj učesnika ({_viewModel.NewPart.NumberOfPeople})");
                ShowFieldError(ParticipantFirstNameTextBox, $"Već ste dodali maksimalan broj učesnika ({_viewModel.NewPart.NumberOfPeople})");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(_viewModel?.NewParticipant?.FirstName))
            {
                errorMessages.Add("• Ime je obavezno");
                ShowFieldError(ParticipantFirstNameTextBox, "Ime je obavezno");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(_viewModel?.NewParticipant?.LastName))
            {
                errorMessages.Add("• Prezime je obavezno");
                ShowFieldError(ParticipantLastNameTextBox, "Prezime je obavezno");
                isValid = false;
            }

            if (_viewModel?.NewParticipant?.Age <= 0 || _viewModel?.NewParticipant?.Age > 120)
            {
                errorMessages.Add("• Godine moraju biti između 1 i 120");
                ShowFieldError(ParticipantAgeTextBox, "Godine moraju biti između 1 i 120");
                isValid = false;
            }

            if (!isValid)
            {
                string combinedErrors = "Greške kod učesnika:\n\n" + string.Join("\n", errorMessages);
                ShowErrorOverlay(combinedErrors);
            }

            return isValid;
        }

        private void ShowFieldError(TextBox textBox, string message)
        {
            if (textBox != null)
            {
                textBox.Style = (Style)FindResource("ErrorTextBoxStyle");
                textBox.ToolTip = message;
            }
        }

        private void HideFieldError(TextBox textBox)
        {
            if (textBox != null)
            {
                textBox.Style = (Style)FindResource("ValidationTextBoxStyle");
                textBox.ToolTip = null;
            }
        }

        private void ClearAllErrors()
        {
            HideFieldError(CityTextBox);
            HideFieldError(CountryTextBox);
            HideFieldError(DescriptionTextBox);
            HideFieldError(NumberOfPeopleTextBox);
            ClearParticipantErrors();
        }

        private void ClearParticipantErrors()
        {
            HideFieldError(ParticipantFirstNameTextBox);
            HideFieldError(ParticipantLastNameTextBox);
            HideFieldError(ParticipantAgeTextBox);
        }

       
        private void ShowSuccessOverlay(string message)
        {
            SuccessMessage.Text = message;
            SuccessOverlay.Visibility = Visibility.Visible;
        }

        private void ShowErrorOverlay(string message)
        {
            ErrorMessage.Text = message;
            ErrorOverlay.Visibility = Visibility.Visible;
        }

        private void CloseSuccessOverlay(object sender, RoutedEventArgs e)
        {
            SuccessOverlay.Visibility = Visibility.Collapsed;
        }

        private void CloseErrorOverlay(object sender, RoutedEventArgs e)
        {
            ErrorOverlay.Visibility = Visibility.Collapsed;
        }
    }

    
    public static class UIHelper
    {
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = LogicalTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;

            return FindParent<T>(parentObject);
        }
    }

    
    public class ComplexTourRequestEventArgs : EventArgs
    {
        public object TourRequest { get; set; }

        public ComplexTourRequestEventArgs(object tourRequest)
        {
            TourRequest = tourRequest;
        }
    }
}