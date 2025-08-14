using BookingApp.Domain;
using BookingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookingApp.Presentation.Owner
{
    /// <summary>
    /// Interaction logic for RegisterAccommodationView.xaml
    /// </summary>
    public partial class RegisterAccommodationView : UserControl
    {
        public AccommodationRepository AccommodationRepository { get; set; }
        public Accommodation Accommodation { get; set; }
        public Location Location { get; set; }
        public AccommodationImageRepository AccommodationImageRepository { get; set; }
        public AccommodationImage AccommodationImage { get; set; }

        public LocationRepository LocationRepository { get; set; }

        public RegisterAccommodationView()
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
            ClearForm();
        }
        private void ClearForm()
        {
            Accommodation = new Accommodation
            {
                GeoLocation = new Location(),
                Images = new List<AccommodationImage>()
            };
            DataContext = null;
            DataContext = this;
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

