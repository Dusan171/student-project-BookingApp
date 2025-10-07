using System.Windows;
using System.Windows.Controls;
using BookingApp.Presentation.ViewModel.Tourist;
using BookingApp.Services.DTO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace BookingApp.Presentation.View.Tourist
{
    public partial class TourReservationView : UserControl
    {
        public TourReservationViewModel ViewModel { get; private set; }
        public event System.Action ReservationCompleted;
        public event System.Action ReservationCancelled;

        private System.Windows.Threading.DispatcherTimer _successTimer;

        public TourReservationView()
        {
            InitializeComponent();
        }

        public void SetTour(TourDTO tourDto)
        {
            if (tourDto == null) return;

            // VAŽNO: Očisti sve overlays kada se postavlja nova tura
            ClearAllOverlays();

            ViewModel = new TourReservationViewModel(tourDto);
            DataContext = ViewModel;
            SetupEventHandlers();
        }

        // Dodaj metodu za čišćenje overlay-a
        private void ClearAllOverlays()
        {
            // Zaustavi timer ako je aktivan
            _successTimer?.Stop();
            _successTimer = null;

            SuccessOverlay.Visibility = Visibility.Collapsed;
            ErrorOverlay.Visibility = Visibility.Collapsed;
            SuccessMessage.Text = "";
            ErrorMessage.Text = "";
        }

        private void SetupEventHandlers()
        {
            if (ViewModel != null)
            {
                ViewModel.ReservationCompleted += OnReservationCompleted;
                ViewModel.ReservationFailed += OnReservationFailed;
            }
        }

        private void OnReservationCompleted()
        {
            ShowSuccessOverlay($"Tura '{ViewModel?.TourName}' je uspešno rezervisana!");

            // Zaustavi postojeći timer ako postoji
            _successTimer?.Stop();

            _successTimer = new System.Windows.Threading.DispatcherTimer();
            _successTimer.Interval = System.TimeSpan.FromSeconds(3); // Povećaj na 3 sekunde
            _successTimer.Tick += (s, e) => {
                _successTimer.Stop();
                CloseSuccessOverlay(null, null); // Automatski zatvori
                ReservationCompleted?.Invoke();
            };
            _successTimer.Start();
        }

        private void OnReservationFailed(string errorMessage)
        {
            ShowErrorOverlay(errorMessage);
        }

        // Real-time validacija polja
        private void ValidateGuestField(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string fieldName = textBox.Name;
            string value = textBox.Text?.Trim();

            var parentBorder = FindParent<Border>(textBox);
            if (parentBorder == null) return;

            switch (fieldName)
            {
                case "FirstNameTextBox":
                    var firstNameError = FindChildByName<TextBlock>(parentBorder, "FirstNameError");
                    if (firstNameError != null)
                    {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            ShowFieldError(textBox, firstNameError, "Ime je obavezno");
                        }
                        else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                        {
                            ShowFieldError(textBox, firstNameError, "Ime može sadržavati samo slova");
                        }
                        else if (value.Length > 50)
                        {
                            ShowFieldError(textBox, firstNameError, "Ime je predugačko (maks. 50 karaktera)");
                        }
                        else
                        {
                            HideFieldError(textBox, firstNameError);
                        }
                    }
                    break;

                case "LastNameTextBox":
                    var lastNameError = FindChildByName<TextBlock>(parentBorder, "LastNameError");
                    if (lastNameError != null)
                    {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            ShowFieldError(textBox, lastNameError, "Prezime je obavezno");
                        }
                        else if (!Regex.IsMatch(value, @"^[a-žA-Ž\s\-']+$"))
                        {
                            ShowFieldError(textBox, lastNameError, "Prezime može sadržavati samo slova");
                        }
                        else if (value.Length > 50)
                        {
                            ShowFieldError(textBox, lastNameError, "Prezime je predugačko (maks. 50 karaktera)");
                        }
                        else
                        {
                            HideFieldError(textBox, lastNameError);
                        }
                    }
                    break;

                case "AgeTextBox":
                    var ageError = FindChildByName<TextBlock>(parentBorder, "AgeError");
                    if (ageError != null)
                    {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            ShowFieldError(textBox, ageError, "Godine su obavezne");
                        }
                        else if (!int.TryParse(value, out int age) || age <= 0)
                        {
                            ShowFieldError(textBox, ageError, "Godine moraju biti pozitivan broj");
                        }
                        else if (age > 120)
                        {
                            ShowFieldError(textBox, ageError, "Maksimalno 120 godina");
                        }
                        else
                        {
                            HideFieldError(textBox, ageError);
                        }
                    }
                    break;

                case "EmailTextBox":
                    var emailError = FindChildByName<TextBlock>(parentBorder, "EmailError");
                    if (emailError != null)
                    {
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            ShowFieldError(textBox, emailError, "Email je obavezan za glavni kontakt");
                        }
                        else if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        {
                            ShowFieldError(textBox, emailError, "Neispravna email adresa");
                        }
                        else if (value.Length > 100)
                        {
                            ShowFieldError(textBox, emailError, "Email je predugačak");
                        }
                        else
                        {
                            HideFieldError(textBox, emailError);
                        }
                    }
                    break;
            }
        }

        // Klik na potvrdi rezervaciju
        private void ConfirmReservation_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAllFields())
            {
                return; // Greška je već prikazana u ValidateAllFields
            }

            // Pozovi ViewModel da izvrši rezervaciju
            if (ViewModel?.ConfirmReservationCommand?.CanExecute(null) == true)
            {
                ViewModel.ConfirmReservationCommand.Execute(null);
            }
        }

        private bool ValidateAllFields()
        {
            bool isValid = true;
            var errorMessages = new List<string>();

            if (ViewModel?.GuestDetails == null || ViewModel.GuestDetails.Count == 0)
            {
                errorMessages.Add("• Morate dodati barem jednog učesnika");
                isValid = false;
            }
            else
            {
                for (int i = 0; i < ViewModel.GuestDetails.Count; i++)
                {
                    var guest = ViewModel.GuestDetails[i];
                    string guestTitle = guest.IsMainContact ? "glavni kontakt" : $"učesnik {i + 1}";

                    if (string.IsNullOrWhiteSpace(guest.FirstName))
                    {
                        errorMessages.Add($"• Ime je obavezno za {guestTitle}");
                        isValid = false;
                    }

                    if (string.IsNullOrWhiteSpace(guest.LastName))
                    {
                        errorMessages.Add($"• Prezime je obavezno za {guestTitle}");
                        isValid = false;
                    }

                    if (guest.Age <= 0 || guest.Age > 120)
                    {
                        errorMessages.Add($"• Godine moraju biti između 1 i 120 za {guestTitle}");
                        isValid = false;
                    }

                    if (guest.IsMainContact && string.IsNullOrWhiteSpace(guest.Email))
                    {
                        errorMessages.Add($"• Email je obavezan za {guestTitle}");
                        isValid = false;
                    }
                    else if (guest.IsMainContact && !string.IsNullOrWhiteSpace(guest.Email) &&
                             !Regex.IsMatch(guest.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        errorMessages.Add($"• Neispravna email adresa za {guestTitle}");
                        isValid = false;
                    }
                }
            }

            if (!isValid && errorMessages.Count > 0)
            {
                string combinedErrors = "Molimo ispravite sledeće greške:\n\n" + string.Join("\n", errorMessages);
                ShowErrorOverlay(combinedErrors);
            }

            return isValid;
        }

        // Helper metode za validaciju
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

        // Metode za pronalaženje UI elemenata
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);

            if (parent == null) return null;

            if (parent is T parentAsT)
                return parentAsT;
            else
                return FindParent<T>(parent);
        }

        private T FindChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is T element && element.Name == name)
                    return element;

                var foundChild = FindChildByName<T>(child, name);
                if (foundChild != null)
                    return foundChild;
            }
            return null;
        }

        // Web-like overlay metode
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationCancelled?.Invoke();
        }

        public void ClearForm()
        {
            // Očisti overlays pre brisanja ViewModel-a
            ClearAllOverlays();

            if (ViewModel != null)
            {
                ViewModel.ReservationCompleted -= OnReservationCompleted;
                ViewModel.ReservationFailed -= OnReservationFailed;
                ViewModel.Dispose();
                ViewModel = null;
            }
            DataContext = null;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}