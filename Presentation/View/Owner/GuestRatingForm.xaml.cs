using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.ViewModel.Owner;
using BookingApp.Repositories;
using BookingApp.Services;
using System;
using System.Windows;

namespace BookingApp.Presentation.View.Owner
{
    public partial class GuestRatingForm : Window
    {
        private readonly GuestRatingViewModel _viewModel;

        public GuestRatingForm(int reservationId, int guestId)
        {
            InitializeComponent();

            _viewModel = new GuestRatingViewModel(reservationId, guestId);
            _viewModel.RatingSaved += (s, e) => this.Close();

            DataContext = _viewModel;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.TryParseRatings(CleanlinessBox.Text, RuleRespectBox.Text, out _, out _))
                return;

            _viewModel.GuestReview.Comment = CommentBox.Text;
            _viewModel.SaveRating();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
