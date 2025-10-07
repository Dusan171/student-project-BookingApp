using System.Windows.Controls;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services;
using System.Windows;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourRequestsView : UserControl
    {
        private TourRequestViewModel _viewModel;

        public TourRequestsView()
        {
            InitializeComponent();
        }

        public void InitializeViewModel(int userId)
        {
            var requestService = Injector.CreateInstance<ITourRequestService>();
            _viewModel = new TourRequestViewModel(requestService, userId);
            DataContext = _viewModel;
        }

        private void CreateRequest_Click(object sender, RoutedEventArgs e)
        {
            CreateRequestView.Visibility = Visibility.Visible;
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

            if (!ValidateParticipants())
            {
                return;
            }

            try
            {
                
                SaveRequestSilent();

                ShowSuccessOverlay("Zahtev za turu je uspešno kreiran!");
                CreateRequestView.Visibility = Visibility.Collapsed;
                ClearAllErrors();
            }
            catch (Exception ex)
            {
                ShowErrorOverlay($"Greška pri kreiranju zahteva: {ex.Message}");
            }
        }

        private void SaveRequestSilent()
        {
           
            var newRequest = _viewModel.NewRequest;
            newRequest.Participants = _viewModel.Participants.ToList();
            newRequest.NumberOfPeople = _viewModel.Participants.Count;

            var requestService = Injector.CreateInstance<ITourRequestService>();
            var savedRequest = requestService.CreateRequest(newRequest);

           
            _viewModel.FilteredRequests.Insert(0, savedRequest);

            
            _viewModel.NewRequest = new Services.DTO.TourRequestDTO
            {
                TouristId = _viewModel.NewRequest.TouristId,
                DateFrom = DateTime.Now.AddDays(3),
                DateTo = DateTime.Now.AddDays(10),
                NumberOfPeople = 1,
                Participants = new System.Collections.Generic.List<Services.DTO.TourRequestParticipantDTO>()
            };
            _viewModel.NewParticipant = new Services.DTO.TourRequestParticipantDTO();
            _viewModel.Participants.Clear();
        }

        private void AddParticipant_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateParticipantFields())
            {
                return;
            }

            try
            {
                _viewModel.Participants.Add(new Services.DTO.TourRequestParticipantDTO
                {
                    FirstName = _viewModel.NewParticipant.FirstName,
                    LastName = _viewModel.NewParticipant.LastName,
                    Age = _viewModel.NewParticipant.Age
                });

                
                _viewModel.NewParticipant = new Services.DTO.TourRequestParticipantDTO();
                ClearParticipantErrors();
            }
            catch (Exception ex)
            {
                ShowErrorOverlay($"Greška pri dodavanju učesnika: {ex.Message}");
            }
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
                        ShowFieldError(textBox, CityError, "Grad je obavezan");
                    }
                    else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                    {
                        ShowFieldError(textBox, CityError, "Grad može sadržavati samo slova");
                    }
                    else if (value.Length > 50)
                    {
                        ShowFieldError(textBox, CityError, "Naziv grada je predugačak");
                    }
                    else
                    {
                        HideFieldError(textBox, CityError);
                    }
                    break;

                case "CountryTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, CountryError, "Država je obavezna");
                    }
                    else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                    {
                        ShowFieldError(textBox, CountryError, "Država može sadržavati samo slova");
                    }
                    else if (value.Length > 50)
                    {
                        ShowFieldError(textBox, CountryError, "Naziv države je predugačak");
                    }
                    else
                    {
                        HideFieldError(textBox, CountryError);
                    }
                    break;

                case "DescriptionTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, DescriptionError, "Opis je obavezan");
                    }
                    else if (value.Length < 10)
                    {
                        ShowFieldError(textBox, DescriptionError, "Opis mora imati najmanje 10 karaktera");
                    }
                    else if (value.Length > 500)
                    {
                        ShowFieldError(textBox, DescriptionError, "Opis je predugačak (maks. 500 karaktera)");
                    }
                    else
                    {
                        HideFieldError(textBox, DescriptionError);
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
                    ShowFieldError(null, LanguageError, "Jezik je obavezan");
                }
                else
                {
                    HideFieldError(null, LanguageError);
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
                        ShowFieldError(null, DateFromError, "Datum početka je obavezan");
                    }
                    else if (datePicker.SelectedDate <= DateTime.Now.AddDays(2))
                    {
                        ShowFieldError(null, DateFromError, "Datum mora biti najmanje 3 dana unapred");
                    }
                    else
                    {
                        HideFieldError(null, DateFromError);
                        // Proveri i datum završetka ako postoji
                        if (DateToPicker.SelectedDate.HasValue)
                        {
                            ValidateDateRange();
                        }
                    }
                    break;

                case "DateToPicker":
                    if (!datePicker.SelectedDate.HasValue)
                    {
                        ShowFieldError(null, DateToError, "Datum završetka je obavezan");
                    }
                    else
                    {
                        HideFieldError(null, DateToError);
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
                    ShowFieldError(null, DateToError, "Datum završetka mora biti nakon početka");
                }
                else
                {
                    HideFieldError(null, DateToError);
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
                        ShowFieldError(textBox, ParticipantFirstNameError, "Ime je obavezno");
                    }
                    else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                    {
                        ShowFieldError(textBox, ParticipantFirstNameError, "Ime može sadržavati samo slova");
                    }
                    else
                    {
                        HideFieldError(textBox, ParticipantFirstNameError);
                    }
                    break;

                case "ParticipantLastNameTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, ParticipantLastNameError, "Prezime je obavezno");
                    }
                    else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                    {
                        ShowFieldError(textBox, ParticipantLastNameError, "Prezime može sadržavati samo slova");
                    }
                    else
                    {
                        HideFieldError(textBox, ParticipantLastNameError);
                    }
                    break;

                case "ParticipantAgeTextBox":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ShowFieldError(textBox, ParticipantAgeError, "Godine su obavezne");
                    }
                    else if (!int.TryParse(value, out int age) || age <= 0)
                    {
                        ShowFieldError(textBox, ParticipantAgeError, "Godine moraju biti pozitivan broj");
                    }
                    else if (age > 120)
                    {
                        ShowFieldError(textBox, ParticipantAgeError, "Maksimalno 120 godina");
                    }
                    else
                    {
                        HideFieldError(textBox, ParticipantAgeError);
                    }
                    break;
            }
        }

       
        private bool ValidateAllFields()
        {
            bool isValid = true;
            var errorMessages = new List<string>();

            
            if (string.IsNullOrWhiteSpace(_viewModel?.NewRequest?.City))
            {
                errorMessages.Add("• Grad je obavezan");
                ShowFieldError(CityTextBox, CityError, "Grad je obavezan");
                isValid = false;
            }

           
            if (string.IsNullOrWhiteSpace(_viewModel?.NewRequest?.Country))
            {
                errorMessages.Add("• Država je obavezna");
                ShowFieldError(CountryTextBox, CountryError, "Država je obavezna");
                isValid = false;
            }

            
            if (string.IsNullOrWhiteSpace(_viewModel?.NewRequest?.Description))
            {
                errorMessages.Add("• Opis je obavezan");
                ShowFieldError(DescriptionTextBox, DescriptionError, "Opis je obavezan");
                isValid = false;
            }
            else if (_viewModel.NewRequest.Description.Length < 10)
            {
                errorMessages.Add("• Opis mora imati najmanje 10 karaktera");
                ShowFieldError(DescriptionTextBox, DescriptionError, "Opis mora imati najmanje 10 karaktera");
                isValid = false;
            }

            
            if (string.IsNullOrWhiteSpace(_viewModel?.NewRequest?.Language))
            {
                errorMessages.Add("• Jezik je obavezan");
                ShowFieldError(null, LanguageError, "Jezik je obavezan");
                isValid = false;
            }

            if (_viewModel?.NewRequest?.DateFrom == default(DateTime) || _viewModel?.NewRequest?.DateFrom <= DateTime.Now.AddDays(2))
            {
                if (_viewModel?.NewRequest?.DateFrom == default(DateTime))
                {
                    errorMessages.Add("• Datum početka je obavezan");
                    ShowFieldError(null, DateFromError, "Datum početka je obavezan");
                }
                else
                {
                    errorMessages.Add("• Datum početka mora biti najmanje 3 dana unapred");
                    ShowFieldError(null, DateFromError, "Datum mora biti najmanje 3 dana unapred");
                }
                isValid = false;
            }

            if (_viewModel?.NewRequest?.DateTo == default(DateTime) || _viewModel?.NewRequest?.DateTo <= _viewModel?.NewRequest?.DateFrom)
            {
                if (_viewModel?.NewRequest?.DateTo == default(DateTime))
                {
                    errorMessages.Add("• Datum završetka je obavezan");
                    ShowFieldError(null, DateToError, "Datum završetka je obavezan");
                }
                else
                {
                    errorMessages.Add("• Datum završetka mora biti nakon početka");
                    ShowFieldError(null, DateToError, "Datum završetka mora biti nakon početka");
                }
                isValid = false;
            }

            if (!isValid)
            {
                string combinedErrors = "Molimo ispravite sledeće greške:\n\n" + string.Join("\n", errorMessages);
                ShowErrorOverlay(combinedErrors);
            }

            return isValid;
        }

        private bool ValidateParticipants()
        {
            if (_viewModel?.Participants?.Count == 0)
            {
                ShowErrorOverlay("Dodajte najmanje jednog učesnika.");
                return false;
            }
            return true;
        }

        private bool ValidateParticipantFields()
        {
            bool isValid = true;
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(_viewModel?.NewParticipant?.FirstName))
            {
                errorMessages.Add("• Ime je obavezno");
                ShowFieldError(ParticipantFirstNameTextBox, ParticipantFirstNameError, "Ime je obavezno");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(_viewModel?.NewParticipant?.LastName))
            {
                errorMessages.Add("• Prezime je obavezno");
                ShowFieldError(ParticipantLastNameTextBox, ParticipantLastNameError, "Prezime je obavezno");
                isValid = false;
            }

            if (_viewModel?.NewParticipant?.Age <= 0 || _viewModel?.NewParticipant?.Age > 120)
            {
                errorMessages.Add("• Godine moraju biti između 1 i 120");
                ShowFieldError(ParticipantAgeTextBox, ParticipantAgeError, "Godine moraju biti između 1 i 120");
                isValid = false;
            }

            if (!isValid)
            {
                string combinedErrors = "Greške kod učesnika:\n\n" + string.Join("\n", errorMessages);
                ShowErrorOverlay(combinedErrors);
            }

            return isValid;
        }

        
        private void ShowFieldError(TextBox textBox, TextBlock errorTextBlock, string message)
        {
            if (textBox != null)
            {
                textBox.Style = (Style)FindResource("ErrorTextBoxStyle");
            }

            if (errorTextBlock != null)
            {
                errorTextBlock.Text = message;
                errorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void HideFieldError(TextBox textBox, TextBlock errorTextBlock)
        {
            if (textBox != null)
            {
                textBox.Style = (Style)FindResource("ValidationTextBoxStyle");
            }

            if (errorTextBlock != null)
            {
                errorTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void ClearAllErrors()
        {
            HideFieldError(CityTextBox, CityError);
            HideFieldError(CountryTextBox, CountryError);
            HideFieldError(DescriptionTextBox, DescriptionError);
            HideFieldError(null, LanguageError);
            HideFieldError(null, DateFromError);
            HideFieldError(null, DateToError);
            ClearParticipantErrors();
        }

        private void ClearParticipantErrors()
        {
            HideFieldError(ParticipantFirstNameTextBox, ParticipantFirstNameError);
            HideFieldError(ParticipantLastNameTextBox, ParticipantLastNameError);
            HideFieldError(ParticipantAgeTextBox, ParticipantAgeError);
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
}