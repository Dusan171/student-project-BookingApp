using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BookingApp.Domain;
using BookingApp.Domain.Model;
using BookingApp.Repositories;
using BookingApp.Utilities;

namespace BookingApp.View
{
    public partial class TourSearch : Window
    {
        private readonly TourRepository _tourRepository;
        private readonly TourReservationRepository _reservationRepository;
        private readonly TourReservationRepository _tourReservationRepository; 
        private readonly ReservationGuestRepository _guestRepository;
        private readonly TourReviewRepository _reviewRepository;
        private List<Tour> _currentTours;
        private Tour? _selectedTour;
        private User? _currentUser;

        // Review form state
        private Tour? _selectedTourForReview;
        private TourReservation? _selectedReservationForReview;
        private int _guideKnowledgeRating = 0;
        private int _guideLanguageRating = 0;
        private int _tourInterestRating = 0;
        private List<string> _selectedImagePaths = new List<string>();

        public TourSearch()
        {
            InitializeComponent();
            _tourRepository = new TourRepository();
            _reservationRepository = new TourReservationRepository();
            _tourReservationRepository = new TourReservationRepository(); 
            _guestRepository = new ReservationGuestRepository();
            _reviewRepository = new TourReviewRepository();
            _currentTours = _tourRepository.GetAll() ?? new List<Tour>();
            _currentUser = Session.CurrentUser;

            InitializeInterface();
        }

        public TourSearch(User user) : this()
        {
            _currentUser = user;
        }

        private void InitializeInterface()
        {
            DisplayTours(_currentTours);
            LoadMyReservations();
            LoadCompletedTours(); 
            ResetReservationPanel();
        }

        private void LoadCompletedTours()
        {
            pnlCompletedTours.Children.Clear();

            if (_currentUser == null)
            {
                var noUserPanel = new StackPanel();
                noUserPanel.Children.Add(new TextBlock
                {
                    Text = "Morate se prijaviti da biste videli ture za ocenjivanje.",
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(10)
                });
                pnlCompletedTours.Children.Add(noUserPanel);
                return;
            }

            var completedReservations = _tourReservationRepository.GetCompletedUnreviewedReservationsByTourist(_currentUser.Id);

            if (completedReservations == null || completedReservations.Count == 0)
            {
                var noToursPanel = new StackPanel();
                noToursPanel.Children.Add(new TextBlock
                {
                    Text = "Nemate završenih tura za ocenjivanje.",
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(10)
                });
                pnlCompletedTours.Children.Add(noToursPanel);
                return;
            }

            foreach (var reservation in completedReservations)
            {
                var tourPanel = CreateCompletedTourPanel(reservation);
                pnlCompletedTours.Children.Add(tourPanel);
            }
        }

        private Border CreateCompletedTourPanel(TourReservation reservation)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 5, 0, 5),
                Padding = new Thickness(10, 8, 10, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Naziv ture
            var tourNameText = new TextBlock
            {
                Text = reservation.TourName,
                FontSize = 11,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(tourNameText, 0);
            grid.Children.Add(tourNameText);

            // Vodič
            var guideText = new TextBlock
            {
                Text = reservation.GuideName,
                FontSize = 11,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(guideText, 1);
            grid.Children.Add(guideText);

            // Datum
            var dateText = new TextBlock
            {
                Text = reservation.TourDateFormatted,
                FontSize = 11,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(dateText, 2);
            grid.Children.Add(dateText);

            // Status
            var statusText = new TextBlock
            {
                Text = "Završeno",
                FontSize = 11,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(statusText, 3);
            grid.Children.Add(statusText);

            
            var reviewButton = new Button
            {
                Content = "OCENI",
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC")),
                Foreground = Brushes.White,
                Padding = new Thickness(8, 4, 8, 4),
                FontSize = 10,
                Tag = reservation
            };
            reviewButton.Click += ReviewButton_Click; 
            Grid.SetColumn(reviewButton, 4);
            grid.Children.Add(reviewButton);

            border.Child = grid;
            return border;
        }

        
        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is TourReservation reservation)
            {
                _selectedReservationForReview = reservation;
                _selectedTourForReview = reservation.Tour;
                ShowReviewForm();
            }
        }

        private void ReviewTourBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tour tour)
            {
                _selectedTourForReview = tour;
                ShowReviewForm();
            }
        }

        /*private void ViewReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tour tour)
            {
                var existingReview = _reviewRepository.GetByTouristId(_currentUser!.Id)
                    .FirstOrDefault(r => r.TourId == tour.Id);

                if (existingReview != null)
                {
                    string imageInfo = existingReview.ImagePaths.Count > 0
                        ? $"\nSlike: {existingReview.ImagePaths.Count} slika priloženo"
                        : "\nNema priloženih slika";

                    MessageBox.Show($"Vaša ocena za turu '{tour.Name}':\n\n" +
                                   $"{existingReview.GetRatingText()}\n" +
                                   $"Komentar: {existingReview.Comment}\n" +
                                   $"Datum: {existingReview.GetFormattedDate()}{imageInfo}",
                                   "Vaša ocena", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }*/

        private void ShowReviewForm()
        {
            if (_selectedTourForReview == null) return;

            lblReviewTourName.Text = $"Ocenjivanje ture: {_selectedTourForReview.Name}";
            reviewFormPanel.Visibility = Visibility.Visible;

            ResetReviewForm();
        }

        private void ResetReviewForm()
        {
            _guideKnowledgeRating = 0;
            _guideLanguageRating = 0;
            _tourInterestRating = 0;
            _selectedImagePaths.Clear();

            ResetStars();
            txtReviewComment.Text = "";
            lblSelectedFile.Text = "No file chosen";
            pnlSelectedImages.Children.Clear();
            selectedImagesScrollViewer.Visibility = Visibility.Collapsed;

            lblGuideKnowledgeRating.Text = "(1-5)";
            lblGuideLanguageRating.Text = "(1-5)";
            lblTourInterestRating.Text = "(1-5)";
        }

        private void ResetStars()
        {
            for (int i = 1; i <= 5; i++)
            {
                var img = FindChild<Image>(pnlGuideKnowledgeRating, $"img{i}GuideKnowledge");
                if (img != null)
                {
                    try
                    {
                        img.Source = new BitmapImage(new Uri("../Resources/Images/star_empty.png", UriKind.Relative));
                    }
                    catch { }
                }
            }

            for (int i = 1; i <= 5; i++)
            {
                var img = FindChild<Image>(pnlGuideLanguageRating, $"img{i}GuideLanguage");
                if (img != null)
                {
                    try
                    {
                        img.Source = new BitmapImage(new Uri("../Resources/Images/star_empty.png", UriKind.Relative));
                    }
                    catch { }
                }
            }

            for (int i = 1; i <= 5; i++)
            {
                var img = FindChild<Image>(pnlTourInterestRating, $"img{i}TourInterest");
                if (img != null)
                {
                    try
                    {
                        img.Source = new BitmapImage(new Uri("../Resources/Images/star_empty.png", UriKind.Relative));
                    }
                    catch { }
                }
            }
        }

        private void StarGuideKnowledge_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int rating))
            {
                _guideKnowledgeRating = rating;
                UpdateStarDisplay(pnlGuideKnowledgeRating, "GuideKnowledge", rating);
                lblGuideKnowledgeRating.Text = $"({rating}/5)";
            }
        }

        private void StarGuideLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int rating))
            {
                _guideLanguageRating = rating;
                UpdateStarDisplay(pnlGuideLanguageRating, "GuideLanguage", rating);
                lblGuideLanguageRating.Text = $"({rating}/5)";
            }
        }

        private void StarTourInterest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int rating))
            {
                _tourInterestRating = rating;
                UpdateStarDisplay(pnlTourInterestRating, "TourInterest", rating);
                lblTourInterestRating.Text = $"({rating}/5)";
            }
        }

        private void UpdateStarDisplay(StackPanel panel, string prefix, int rating)
        {
            for (int i = 1; i <= 5; i++)
            {
                var img = FindChild<Image>(panel, $"img{i}{prefix}");
                if (img != null)
                {
                    try
                    {
                        if (i <= rating)
                        {
                            img.Source = new BitmapImage(new Uri("../Resources/Images/star_filled.png", UriKind.Relative));
                        }
                        else
                        {
                            img.Source = new BitmapImage(new Uri("../Resources/Images/star_empty.png", UriKind.Relative));
                        }
                    }
                    catch { }
                }
            }
        }

        private void BtnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Izaberite sliku",
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    if (!_selectedImagePaths.Contains(fileName))
                    {
                        _selectedImagePaths.Add(fileName);
                    }
                }

                UpdateSelectedImagesDisplay();
            }
        }

        private void UpdateSelectedImagesDisplay()
        {
            pnlSelectedImages.Children.Clear();

            if (_selectedImagePaths.Count == 0)
            {
                lblSelectedFile.Text = "No file chosen";
                selectedImagesScrollViewer.Visibility = Visibility.Collapsed;
                return;
            }

            lblSelectedFile.Text = $"{_selectedImagePaths.Count} slika izabrano";
            selectedImagesScrollViewer.Visibility = Visibility.Visible;

            foreach (var imagePath in _selectedImagePaths)
            {
                Border imageBorder = new Border
                {
                    Width = 60,
                    Height = 60,
                    Margin = new Thickness(0, 0, 5, 0),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(3)
                };

                Grid imageGrid = new Grid();

                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.DecodePixelWidth = 60;
                    bitmap.EndInit();

                    Image img = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.UniformToFill
                    };
                    imageGrid.Children.Add(img);
                }
                catch
                {
                    TextBlock errorText = new TextBlock
                    {
                        Text = "X",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 20,
                        Foreground = Brushes.Red
                    };
                    imageGrid.Children.Add(errorText);
                }

                Button removeBtn = new Button
                {
                    Content = "×",
                    Width = 20,
                    Height = 20,
                    Background = new SolidColorBrush(Color.FromArgb(180, 255, 0, 0)),
                    Foreground = Brushes.White,
                    BorderBrush = Brushes.Transparent,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 2, 2, 0),
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Tag = imagePath
                };

                removeBtn.Click += (s, e) =>
                {
                    if (removeBtn.Tag is string pathToRemove)
                    {
                        _selectedImagePaths.Remove(pathToRemove);
                        UpdateSelectedImagesDisplay();
                    }
                };

                imageGrid.Children.Add(removeBtn);
                imageBorder.Child = imageGrid;
                pnlSelectedImages.Children.Add(imageBorder);
            }
        }

        private void BtnCancelReview_Click(object sender, RoutedEventArgs e)
        {
            reviewFormPanel.Visibility = Visibility.Collapsed;
            _selectedTourForReview = null;
            _selectedReservationForReview = null;
        }

        private void BtnSubmitReview_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTourForReview == null || _currentUser == null)
            {
                MessageBox.Show("Greška u podacima.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_guideKnowledgeRating == 0 || _guideLanguageRating == 0 || _tourInterestRating == 0)
            {
                MessageBox.Show("Molimo ocenite sve kategorije (1-5 zvezdica).", "Nedostaju ocene",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtReviewComment.Text))
            {
                MessageBox.Show("Molimo unesite komentar.", "Nedostaje komentar",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var review = new TourReview
            {
                TourId = _selectedTourForReview.Id,
                TouristId = _currentUser.Id,
                GuideKnowledge = _guideKnowledgeRating,
                GuideLanguage = _guideLanguageRating,
                TourInterest = _tourInterestRating,
                Comment = txtReviewComment.Text.Trim(),
                ImagePaths = new List<string>(_selectedImagePaths),
                Date = DateTime.Now
            };

            try
            {
                _reviewRepository.AddReview(review);
                MessageBox.Show("Ocena je uspešno sačuvana!", "Uspeh",
                               MessageBoxButton.OK, MessageBoxImage.Information);

                reviewFormPanel.Visibility = Visibility.Collapsed;
                _selectedTourForReview = null;
                _selectedReservationForReview = null;
                LoadCompletedTours();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri čuvanju ocene: {ex.Message}", "Greška",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       

        private void ResetReservationPanel()
        {
            lblTourName.Text = "Izaberite turu za rezervaciju";
            lblTourName.Foreground = Brushes.Gray;
            lblTourDetails.Text = "Kliknite na 'REZERVIŠI' dugme kod bilo koje ture";
            lblTourDetails.Foreground = Brushes.Gray;
            lblAvailableSpots.Text = "";

            peopleCountPanel.IsEnabled = false;
            guestDetailsPanel.IsEnabled = false;
            buttonPanel.IsEnabled = false;

            pnlGuestDetails.Children.Clear();
            txtModalBrojOsoba.Text = "1";

            _selectedTour = null;
        }

        private void EnableReservationPanel()
        {
            peopleCountPanel.IsEnabled = true;
            guestDetailsPanel.IsEnabled = true;
            buttonPanel.IsEnabled = true;

            lblTourName.Foreground = Brushes.Black;
            lblTourDetails.Foreground = Brushes.Gray;
        }

        private void BtnPretrazi_Click(object sender, RoutedEventArgs e)
        {
            string? city = string.IsNullOrWhiteSpace(txtGrad.Text) || txtGrad.Text == "npr. Beograd" ? null : txtGrad.Text;
            string? country = string.IsNullOrWhiteSpace(txtDrzava.Text) || txtDrzava.Text == "npr. Srbija" ? null : txtDrzava.Text;
            string? language = null;

            if (cmbJezik.SelectedItem is ComboBoxItem selectedItem)
            {
                language = selectedItem.Content?.ToString();
            }

            double? duration = null;
            if (!string.IsNullOrWhiteSpace(txtTrajanje.Text) && double.TryParse(txtTrajanje.Text, out double d))
                duration = d;

            int? maxPeople = null;
            if (!string.IsNullOrWhiteSpace(txtBrojLjudi.Text) && int.TryParse(txtBrojLjudi.Text, out int p))
                maxPeople = p;

            _currentTours = _tourRepository.SearchTours(city ?? "", country ?? "", language ?? "", maxPeople, duration) ?? new List<Tour>();
            DisplayTours(_currentTours);
        }

        private void DisplayTours(List<Tour> tours)
        {
            pnlResults.Children.Clear();

            if (tours.Count == 0)
            {
                var noResultsText = new TextBlock
                {
                    Text = "Nema dostupnih tura za zadate kriterijume.",
                    FontSize = 14,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };
                pnlResults.Children.Add(noResultsText);
                return;
            }

            foreach (var tour in tours)
            {
                Border border = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(15, 15, 15, 15),
                    CornerRadius = new CornerRadius(5)
                };

                StackPanel sp = new StackPanel();

                sp.Children.Add(new TextBlock
                {
                    Text = tour.Name ?? "Nepoznata tura",
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 5)
                });

                string locationText = tour.Location != null ? $"{tour.Location.City}, {tour.Location.Country}" : "Nepoznata lokacija";

                StackPanel locationPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 5) };

                try
                {
                    locationPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("../Resources/Images/location.png", UriKind.Relative)),
                        Width = 12,
                        Height = 12,
                        Margin = new Thickness(0, 0, 3, 0)
                    });
                }
                catch (Exception) { }

                locationPanel.Children.Add(new TextBlock { Text = $"{locationText} | ", FontSize = 11, Foreground = Brushes.Gray });

                try
                {
                    locationPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("../Resources/Images/clock.png", UriKind.Relative)),
                        Width = 12,
                        Height = 12,
                        Margin = new Thickness(5, 0, 3, 0)
                    });
                }
                catch (Exception) { }

                locationPanel.Children.Add(new TextBlock { Text = $"{tour.DurationHours}h | ", FontSize = 11, Foreground = Brushes.Gray });

                try
                {
                    locationPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("../Resources/Images/language.png", UriKind.Relative)),
                        Width = 12,
                        Height = 12,
                        Margin = new Thickness(5, 0, 3, 0)
                    });
                }
                catch (Exception) { }

                locationPanel.Children.Add(new TextBlock { Text = tour.Language ?? "Nepoznat jezik", FontSize = 11, Foreground = Brushes.Gray });
                sp.Children.Add(locationPanel);

                StackPanel spotsPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 5) };

                try
                {
                    spotsPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("../Resources/Images/people.png", UriKind.Relative)),
                        Width = 12,
                        Height = 12,
                        Margin = new Thickness(0, 0, 3, 0)
                    });
                }
                catch (Exception) { }

                spotsPanel.Children.Add(new TextBlock
                {
                    Text = $"Slobodnih mesta: {tour.AvailableSpots}/{tour.MaxTourists}",
                    FontSize = 11,
                    Foreground = tour.AvailableSpots > 0 ? Brushes.Green : Brushes.Red,
                    FontWeight = FontWeights.Bold
                });
                sp.Children.Add(spotsPanel);

                sp.Children.Add(new TextBlock
                {
                    Text = $"Opis: {tour.Description ?? "Nema opisa"}",
                    FontSize = 11,
                    Foreground = Brushes.Gray,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                Button reserveBtn = new Button
                {
                    Background = tour.AvailableSpots > 0 ? new SolidColorBrush(Color.FromRgb(46, 134, 193)) : new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                    Foreground = Brushes.White,
                    Padding = new Thickness(15, 8, 15, 8),
                    FontWeight = FontWeights.Bold,
                    BorderBrush = Brushes.Transparent,
                    IsEnabled = true,
                    Tag = tour
                };

                StackPanel buttonContent = new StackPanel { Orientation = Orientation.Horizontal };

                try
                {
                    buttonContent.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri(tour.AvailableSpots > 0 ? "../Resources/Images/reserve.png" : "../Resources/Images/cancel.png", UriKind.Relative)),
                        Width = 16,
                        Height = 16,
                        Margin = new Thickness(0, 0, 5, 0)
                    });
                }
                catch (Exception) { }

                buttonContent.Children.Add(new TextBlock
                {
                    Text = "REZERVIŠI",
                    VerticalAlignment = VerticalAlignment.Center
                });
                reserveBtn.Content = buttonContent;
                reserveBtn.Click += ReserveBtn_Click;

                sp.Children.Add(reserveBtn);
                border.Child = sp;
                pnlResults.Children.Add(border);
            }
        }

        private void ReserveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tour tour)
            {
                _selectedTour = tour;
                ShowReservationPanel();
            }
        }

        private void ShowReservationPanel()
        {
            if (_selectedTour == null) return;

            EnableReservationPanel();

            lblTourName.Text = _selectedTour.Name ?? "Nepoznata tura";
            string locationText = _selectedTour.Location != null ? $"{_selectedTour.Location.City}, {_selectedTour.Location.Country}" : "Nepoznata lokacija";
            lblTourDetails.Text = $"{locationText} | {_selectedTour.DurationHours}h | {_selectedTour.Language ?? "Nepoznat jezik"}";
            lblAvailableSpots.Text = $"Dostupno je još {_selectedTour.AvailableSpots} mesta";

            txtModalBrojOsoba.Text = "1";
            GenerateGuestFields(1);
            ScrollToReservations();
        }

        private void ShowAlternativeTours(Tour originalTour, int requiredSpots)
        {
            var alternativeTours = _tourRepository.GetAlternativeTours(originalTour.Id, requiredSpots) ?? new List<Tour>();

            if (alternativeTours.Count == 0)
            {
                string locationInfo = originalTour.Location != null ?
                    $"{originalTour.Location.City}, {originalTour.Location.Country}" : "nepoznata lokacija";

                MessageBox.Show($"Nažalost, tura '{originalTour.Name}' je popunjena i trenutno nema alternativnih tura na istoj lokaciji ({locationInfo}) sa dovoljno mesta ({requiredSpots} osoba).",
                               "Nema alternativa", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string alternatives = $"Tura '{originalTour.Name ?? "Nepoznata"}' je popunjena.\n\n";
            alternatives += $"Dostupne alternative na istoj lokaciji:\n";

            if (originalTour.Location != null)
            {
                alternatives += $"Lokacija: {originalTour.Location.City}, {originalTour.Location.Country}\n\n";
            }

            for (int i = 0; i < alternativeTours.Count; i++)
            {
                var tour = alternativeTours[i];
                alternatives += $"{i + 1}. {tour.Name ?? "Nepoznata tura"}\n";
                alternatives += $"   - Slobodnih mesta: {tour.AvailableSpots}/{tour.MaxTourists}\n";
                alternatives += $"   - Trajanje: {tour.DurationHours}h\n";
                alternatives += $"   - Jezik: {tour.Language ?? "Nepoznat"}\n\n";
            }

            alternatives += "Da li želite da rezervišete neku od alternativnih tura?";

            var result = MessageBox.Show(alternatives, "Alternativne ture dostupne!",
                                        MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ShowAlternativeToursDialog(alternativeTours);
            }
        }

        private void ShowAlternativeToursDialog(List<Tour> alternativeTours)
        {
            Window alternativeDialog = new Window
            {
                Title = "Izaberite alternativnu turu",
                Width = 650,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245))
            };

            StackPanel mainPanel = new StackPanel { Margin = new Thickness(20, 20, 20, 20) };

            mainPanel.Children.Add(new TextBlock
            {
                Text = "Izaberite alternativnu turu:",
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 15),
                Foreground = new SolidColorBrush(Color.FromRgb(46, 134, 193))
            });

            ListBox toursList = new ListBox
            {
                Height = 280,
                Margin = new Thickness(0, 0, 0, 20),
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1)
            };

            foreach (var tour in alternativeTours)
            {
                Border tourBorder = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(5, 5, 5, 5),
                    Padding = new Thickness(15, 12, 15, 12),
                    CornerRadius = new CornerRadius(5)
                };

                StackPanel tourPanel = new StackPanel();

                tourPanel.Children.Add(new TextBlock
                {
                    Text = tour.Name ?? "Nepoznata tura",
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 5)
                });

                string locationText = tour.Location != null ? $"{tour.Location.City}, {tour.Location.Country}" : "Nepoznata lokacija";

                tourPanel.Children.Add(new TextBlock
                {
                    Text = $"Lokacija: {locationText}",
                    FontSize = 11,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 0, 0, 3)
                });

                tourPanel.Children.Add(new TextBlock
                {
                    Text = $"Mesta: {tour.AvailableSpots}/{tour.MaxTourists} | {tour.DurationHours}h | {tour.Language ?? "Nepoznat"}",
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(46, 134, 193)),
                    FontWeight = FontWeights.SemiBold
                });

                tourBorder.Child = tourPanel;

                ListBoxItem item = new ListBoxItem
                {
                    Content = tourBorder,
                    Tag = tour,
                    Margin = new Thickness(0, 2, 0, 2)
                };
                toursList.Items.Add(item);
            }

            mainPanel.Children.Add(toursList);

            StackPanel buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            Button selectButton = new Button
            {
                Content = "REZERVIŠI IZABRANU TURU",
                Background = new SolidColorBrush(Color.FromRgb(40, 167, 69)),
                Foreground = Brushes.White,
                Padding = new Thickness(20, 10, 20, 10),
                Margin = new Thickness(10, 0, 10, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                BorderBrush = Brushes.Transparent
            };

            Button cancelButton = new Button
            {
                Content = "ODUSTANI",
                Background = new SolidColorBrush(Color.FromRgb(108, 117, 125)),
                Foreground = Brushes.White,
                Padding = new Thickness(20, 10, 20, 10),
                Margin = new Thickness(10, 0, 10, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                BorderBrush = Brushes.Transparent
            };

            selectButton.Click += (s, e) =>
            {
                if (toursList.SelectedItem is ListBoxItem selectedItem && selectedItem.Tag is Tour selectedTour)
                {
                    _selectedTour = selectedTour;
                    alternativeDialog.Close();
                    ShowReservationPanel();

                    MessageBox.Show($"Izabrali ste alternativnu turu: '{selectedTour.Name}'\n\nMolimo unesite podatke za rezervaciju.",
                                   "Alternativna tura izabrana", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Molimo izaberite turu iz liste.", "Izbor potreban",
                                   MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            cancelButton.Click += (s, e) => alternativeDialog.Close();

            buttonsPanel.Children.Add(selectButton);
            buttonsPanel.Children.Add(cancelButton);
            mainPanel.Children.Add(buttonsPanel);

            alternativeDialog.Content = mainPanel;
            alternativeDialog.ShowDialog();
        }

        private void GenerateGuestFields(int numberOfGuests)
        {
            pnlGuestDetails.Children.Clear();

            for (int i = 0; i < numberOfGuests; i++)
            {
                Border guestBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(248, 249, 250)),
                    Padding = new Thickness(10, 10, 10, 10),
                    Margin = new Thickness(0, 0, 0, 10),
                    CornerRadius = new CornerRadius(5)
                };

                StackPanel guestPanel = new StackPanel();

                guestPanel.Children.Add(new TextBlock
                {
                    Text = $"Osoba {i + 1}" + (i == 0 ? " (glavni kontakt)" : ""),
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                StackPanel nameRow = new StackPanel { Orientation = Orientation.Horizontal };

                StackPanel firstNamePanel = new StackPanel { Width = 120, Margin = new Thickness(0, 0, 10, 0) };
                firstNamePanel.Children.Add(new TextBlock { Text = "Ime:", FontSize = 10, Margin = new Thickness(0, 0, 0, 2) });
                TextBox firstNameBox = new TextBox { Name = $"txtFirstName{i}", Padding = new Thickness(5, 5, 5, 5) };
                firstNamePanel.Children.Add(firstNameBox);
                nameRow.Children.Add(firstNamePanel);

                StackPanel lastNamePanel = new StackPanel { Width = 120, Margin = new Thickness(0, 0, 10, 0) };
                lastNamePanel.Children.Add(new TextBlock { Text = "Prezime:", FontSize = 10, Margin = new Thickness(0, 0, 0, 2) });
                TextBox lastNameBox = new TextBox { Name = $"txtLastName{i}", Padding = new Thickness(5, 5, 5, 5) };
                lastNamePanel.Children.Add(lastNameBox);
                nameRow.Children.Add(lastNamePanel);

                StackPanel agePanel = new StackPanel { Width = 80 };
                agePanel.Children.Add(new TextBlock { Text = "Godine:", FontSize = 10, Margin = new Thickness(0, 0, 0, 2) });
                TextBox ageBox = new TextBox { Name = $"txtAge{i}", Padding = new Thickness(5, 5, 5, 5) };
                agePanel.Children.Add(ageBox);
                nameRow.Children.Add(agePanel);

                guestPanel.Children.Add(nameRow);

                if (i == 0)
                {
                    StackPanel emailPanel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };
                    emailPanel.Children.Add(new TextBlock { Text = "Email:", FontSize = 10, Margin = new Thickness(0, 0, 0, 2) });
                    TextBox emailBox = new TextBox { Name = $"txtEmail{i}", Padding = new Thickness(5, 5, 5, 5), Width = 250, HorizontalAlignment = HorizontalAlignment.Left };
                    emailPanel.Children.Add(emailBox);
                    guestPanel.Children.Add(emailPanel);
                }

                guestBorder.Child = guestPanel;
                pnlGuestDetails.Children.Add(guestBorder);
            }
        }

        private void LoadMyReservations()
        {
            pnlMyReservations.Children.Clear();

            if (_currentUser == null)
            {
                pnlMyReservations.Children.Add(new TextBlock
                {
                    Text = "Morate se prijaviti da biste videli rezervacije.",
                    FontSize = 11,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                });
                return;
            }

            try
            {
                var reservations = _reservationRepository.GetReservationsByTourist(_currentUser.Id) ?? new List<TourReservation>();

                if (reservations.Count == 0)
                {
                    pnlMyReservations.Children.Add(new TextBlock
                    {
                        Text = "Nemate aktivnih rezervacija.",
                        FontSize = 11,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 20, 0, 0)
                    });
                    return;
                }

                foreach (var reservation in reservations)
                {
                    var tour = _tourRepository.GetById(reservation.TourId);
                    if (tour == null) continue;

                    Border reservationBorder = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(248, 249, 250)),
                        Padding = new Thickness(15, 15, 15, 15),
                        Margin = new Thickness(0, 0, 0, 10),
                        CornerRadius = new CornerRadius(5)
                    };

                    StackPanel reservationPanel = new StackPanel();

                    reservationPanel.Children.Add(new TextBlock
                    {
                        Text = tour.Name ?? "Nepoznata tura",
                        FontWeight = FontWeights.Bold,
                        FontSize = 14
                    });

                    string locationText = tour.Location != null ? $"{tour.Location.City}, {tour.Location.Country}" : "Nepoznata lokacija";

                    reservationPanel.Children.Add(new TextBlock
                    {
                        Text = $"Lokacija: {locationText}",
                        FontSize = 12,
                        Foreground = Brushes.Gray
                    });

                    reservationPanel.Children.Add(new TextBlock
                    {
                        Text = $"Rezervisano: {reservation.ReservationDate:dd.MM.yyyy HH:mm}",
                        FontSize = 12,
                        Foreground = Brushes.Gray
                    });

                    reservationPanel.Children.Add(new TextBlock
                    {
                        Text = $"Broj gostiju: {reservation.NumberOfGuests}",
                        FontSize = 12,
                        Foreground = Brushes.Blue
                    });

                    var mainGuest = reservation.Guests?.FirstOrDefault();
                    if (mainGuest != null)
                    {
                        reservationPanel.Children.Add(new TextBlock
                        {
                            Text = $"Glavni kontakt: {mainGuest.FirstName} {mainGuest.LastName}",
                            FontSize = 12,
                            Foreground = Brushes.Green
                        });
                    }

                    reservationPanel.Children.Add(new TextBlock
                    {
                        Text = $"Status: {GetReservationStatusText(reservation.Status)}",
                        FontSize = 12,
                        Foreground = GetReservationStatusColor(reservation.Status),
                        FontWeight = FontWeights.Bold
                    });

                    reservationBorder.Child = reservationPanel;
                    pnlMyReservations.Children.Add(reservationBorder);
                }
            }
            catch (Exception ex)
            {
                pnlMyReservations.Children.Add(new TextBlock
                {
                    Text = $"Greška pri učitavanju rezervacija: {ex.Message}",
                    FontSize = 11,
                    Foreground = Brushes.Red,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }
        }

        private string GetReservationStatusText(TourReservationStatus status)
        {
            return status switch
            {
                TourReservationStatus.ACTIVE => "Aktivna",
                TourReservationStatus.COMPLETED => "Završena",
                TourReservationStatus.CANCELLED => "Otkazana",
                _ => "Nepoznato"
            };
        }

        private Brush GetReservationStatusColor(TourReservationStatus status)
        {
            return status switch
            {
                TourReservationStatus.ACTIVE => Brushes.Green,
                TourReservationStatus.COMPLETED => Brushes.Blue,
                TourReservationStatus.CANCELLED => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        private void BtnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtModalBrojOsoba.Text, out int current) && current > 1)
            {
                txtModalBrojOsoba.Text = (current - 1).ToString();
                GenerateGuestFields(current - 1);
            }
        }

        private void BtnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTour == null) return;

            if (int.TryParse(txtModalBrojOsoba.Text, out int current))
            {
                int newValue = current + 1;

                txtModalBrojOsoba.Text = newValue.ToString();
                GenerateGuestFields(newValue);

                if (newValue > _selectedTour.AvailableSpots && _selectedTour.AvailableSpots > 0)
                {
                    MessageBox.Show($"Ova tura ima samo {_selectedTour.AvailableSpots} dostupnih mesta, ali možete pokušati rezervaciju - sistem će ponuditi alternative.",
                                   "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void TxtModalBrojOsoba_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedTour == null) return;

            if (int.TryParse(txtModalBrojOsoba.Text, out int numberOfGuests))
            {
                if (numberOfGuests > 0)
                {
                    GenerateGuestFields(numberOfGuests);
                }
            }
        }

        private void BtnCancelReservation_Click(object sender, RoutedEventArgs e)
        {
            ResetReservationPanel();
        }

        private void BtnPotvrdiRezervaciju_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTour == null) return;

            if (!ValidateGuestData())
                return;

            int numberOfGuests = int.Parse(txtModalBrojOsoba.Text);

            var currentTour = _tourRepository.GetById(_selectedTour.Id);
            if (currentTour == null)
            {
                MessageBox.Show("Greška: Tura nije pronađena.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (currentTour.AvailableSpots < numberOfGuests)
            {
                ShowAlternativeTours(currentTour, numberOfGuests);
                return;
            }

            bool reservationSuccess = _tourRepository.ReserveSpots(_selectedTour.Id, numberOfGuests);

            if (!reservationSuccess)
            {
                MessageBox.Show("Neuspešna rezervacija. Možda je tura u međuvremenu popunjena.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                ShowAlternativeTours(_selectedTour, numberOfGuests);
                return;
            }

            var reservation = new TourReservation
            {
                TourId = _selectedTour.Id,
                TouristId = _currentUser?.Id ?? 0,
                NumberOfGuests = numberOfGuests,
                ReservationDate = DateTime.Now,
                Status = TourReservationStatus.ACTIVE
            };

            var guests = new List<ReservationGuest>();
            for (int i = 0; i < numberOfGuests; i++)
            {
                var firstNameBox = FindChild<TextBox>(pnlGuestDetails, $"txtFirstName{i}");
                var lastNameBox = FindChild<TextBox>(pnlGuestDetails, $"txtLastName{i}");
                var ageBox = FindChild<TextBox>(pnlGuestDetails, $"txtAge{i}");
                var emailBox = i == 0 ? FindChild<TextBox>(pnlGuestDetails, $"txtEmail{i}") : null;
                var touristID = _currentUser?.Id ?? 0;
                var guest = new ReservationGuest
                {
                    FirstName = firstNameBox?.Text ?? "",
                    LastName = lastNameBox?.Text ?? "",
                    Age = int.TryParse(ageBox?.Text, out int age) ? age : 0,
                    Email = emailBox?.Text ?? "",
                    TouristId = touristID
                };

                guests.Add(guest);
            }

            reservation.Guests = guests;

            try
            {
                _reservationRepository.Add(reservation);

                MessageBox.Show("Rezervacija je uspešno kreirana!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);

                ResetReservationPanel();
                RefreshData();
                ScrollToReservations();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri kreiranju rezervacije: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshData()
        {
            _currentTours = _tourRepository.GetAll() ?? new List<Tour>();
            DisplayTours(_currentTours);
            LoadMyReservations();
        }

        private void ScrollToReservations()
        {
            if (pnlMyReservations.Parent is FrameworkElement parentElement)
            {
                parentElement.BringIntoView();
            }
        }

        private bool ValidateGuestData()
        {
            if (!int.TryParse(txtModalBrojOsoba.Text, out int numberOfGuests))
            {
                MessageBox.Show("Broj gostiju nije valjan.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            for (int i = 0; i < numberOfGuests; i++)
            {
                var firstNameBox = FindChild<TextBox>(pnlGuestDetails, $"txtFirstName{i}");
                var lastNameBox = FindChild<TextBox>(pnlGuestDetails, $"txtLastName{i}");
                var ageBox = FindChild<TextBox>(pnlGuestDetails, $"txtAge{i}");
                var emailBox = i == 0 ? FindChild<TextBox>(pnlGuestDetails, $"txtEmail{i}") : null;

                if (firstNameBox == null || string.IsNullOrWhiteSpace(firstNameBox.Text))
                {
                    MessageBox.Show($"Ime za osobu {i + 1} je obavezno.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    firstNameBox?.Focus();
                    return false;
                }

                if (lastNameBox == null || string.IsNullOrWhiteSpace(lastNameBox.Text))
                {
                    MessageBox.Show($"Prezime za osobu {i + 1} je obavezno.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    lastNameBox?.Focus();
                    return false;
                }

                if (ageBox == null || !int.TryParse(ageBox.Text, out int age) || age <= 0 || age > 120)
                {
                    MessageBox.Show($"Godine za osobu {i + 1} moraju biti validne (1-120).", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ageBox?.Focus();
                    return false;
                }

                if (i == 0 && (emailBox == null || string.IsNullOrWhiteSpace(emailBox.Text)))
                {
                    MessageBox.Show("Email je obavezan za glavnog kontakta.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    emailBox?.Focus();
                    return false;
                }
            }

            return true;
        }

        private T? FindChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T element && element.Name == name)
                    return element;

                var result = FindChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void BtnTutorijal_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tutorijal će biti prikazan ovde.");
        }

        private void MenuItemMojProfil_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Otvaranje profila korisnika...", "Moj Profil", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItemPromenaLozinke_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Otvaranje forme za promenu lozinke...", "Promena Lozinke", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItemOdjava_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Da li ste sigurni da se želite odjaviti?", "Odjava",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (Session.CurrentUser != null)
                {
                    Session.CurrentUser = null;
                }

                var loginWindow = new SignInForm();
                loginWindow.Show();
                this.Close();
            }
        }

        private void ProfileIcon_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image profileIcon && profileIcon.ContextMenu != null)
            {
                profileIcon.ContextMenu.IsOpen = true;
            }
        }

        private void BtnOdjava_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new SignInForm();
            loginWindow.Show();
            this.Close();
        }

        private void BtnPretraziteTure_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnRezervacije_Click(object sender, RoutedEventArgs e)
        {
            ScrollToReservations();
        }

        private void BtnOceniTuru_Click(object sender, RoutedEventArgs e)
        {
            if (pnlCompletedTours.Parent is FrameworkElement parentElement)
            {
                parentElement.BringIntoView();
            }
        }

        private void BtnPracenjeTure_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Funkcionalnost praćenja aktivnih tura će biti implementirana.", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnNoviZahtev_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Funkcionalnost novog zahteva će biti implementirana.");
        }

        private void BtnObavestenja_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Funkcionalnost obaveštenja će biti implementirana.");
        }

        private void BtnStatistika_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Statistike funkcionalnost će biti implementirana.");
        }
    }
}