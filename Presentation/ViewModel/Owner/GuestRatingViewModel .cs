using BookingApp.Domain.Interfaces;
using BookingApp.Domain;
using BookingApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BookingApp.Services.DTO;

namespace BookingApp.Presentation.ViewModel.Owner
{
    public class GuestRatingViewModel
    {
        private readonly IGuestReviewService _guestReviewService;

        public GuestReviewDTO GuestReview { get; set; }

        public event EventHandler RatingSaved;
        public event PropertyChangedEventHandler PropertyChanged;

        public GuestRatingViewModel(int reservationId, int guestId)
        {
            GuestReview = new GuestReviewDTO
            {
                Id = guestId,
                ReservationId = reservationId
            };

            _guestReviewService = Injector.CreateInstance<IGuestReviewService>();
        }

        public void SaveRating()
        {
            if (!GuestReview.IsValid(out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _guestReviewService.AddReview(GuestReview);
                RatingSaved?.Invoke(this, EventArgs.Empty);
                MessageBox.Show("Successfully saved guest rating.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unsuccessfully saved guest rating: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool TryParseRatings(string cleanlinessText, string ruleRespectText, out int cleanliness, out int ruleRespect)
        {
            cleanliness = ruleRespect = 0;
            if (!int.TryParse(cleanlinessText, out cleanliness) ||
                !int.TryParse(ruleRespectText, out ruleRespect))
            {
                MessageBox.Show("Please enter valid numbers for ratings.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            GuestReview.CleanlinessRating = cleanliness;
            GuestReview.RuleRespectingRating = ruleRespect;

            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

