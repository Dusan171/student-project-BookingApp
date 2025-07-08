using System.Collections.Generic;
using System;
using System.Windows;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Utilities;

namespace BookingApp.View
{
    /// <summary>
    /// Interaction logic for RegisterAccommodationForm.xaml
    /// </summary>
    public partial class RegisterAccommodationForm : Window
    {
        public AccommodationRepository AccommodationRepository { get; set; }
        public Accommodation Accommodation { get; set; }
        public Location Location { get; set; }
        public AccommodationImageRepository AccommodationImageRepository { get; set; }
        public AccommodationImage AccommodationImage { get; set; }

        public LocationRepository LocationRepository { get; set; }

        public RegisterAccommodationForm()
        {
            InitializeComponent();
            
            Accommodation = new Accommodation
            {
                GeoLocation = new Location(),
                Images = new List<AccommodationImage>()
            };
            AccommodationRepository = new AccommodationRepository();
            LocationRepository = new LocationRepository();
            
            //AccommodationImageRepository = new AccommodationImageRepository();
            DataContext = this;
        }


        public void Logout_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            var signInWindow = new SignInForm();
            signInWindow.Show();

            //Zatvara trenutni prozor
            this.Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (Accommodation.IsValid())
            {
                try
                {
                    var savedLocation = LocationRepository.Save(Accommodation.GeoLocation);
                    Accommodation.GeoLocation = savedLocation;

                    var savedAccommodation = AccommodationRepository.Save(Accommodation);
                    var imageRepository = new AccommodationImageRepository();
                    foreach (var image in Accommodation.Images)
                    {
                        image.AccommodationId = savedAccommodation.Id;
                       // MessageBox.Show($"{image.Path}");
                        imageRepository.Save(image);
                    }

                    MessageBox.Show("Accommodation saved successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while saving: " + ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else 
            {
                MessageBox.Show("Invalid informations!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

      
        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            string imagePath = ImagePathTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(imagePath))
            {
                var newImage = new AccommodationImage
                {
                    Path = imagePath
                };

                Accommodation.Images.Add(newImage);
                ListBoxImages.Items.Refresh();
                ImagePathTextBox.Text = "";
            }
            else
            {
                MessageBox.Show("Please enter a valid image path.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
