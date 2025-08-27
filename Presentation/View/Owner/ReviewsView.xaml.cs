using BookingApp.Domain.Model;
using BookingApp.Domain.Interfaces;
using BookingApp.Presentation.ViewModel.Owner;
using BookingApp.Repositories;
using BookingApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookingApp.Presentation.View.Owner
{

    public partial class ReviewsView : UserControl
    {
        public ReviewsView()
        {
            InitializeComponent();

            var reviewsViewModel = Injector.CreateReviewsViewModel();
            reviewsViewModel.OpenImageGalleryRequested += ShowImageGallery;
            DataContext = reviewsViewModel;
        }
        private void ShowImageGallery(List<string> images)
        {
               
                    var viewModel = Injector.CreateImageGalleryViewModel(images);
                    ImageGallery.DataContext = viewModel;
                    ImageGallery.Visibility = Visibility.Visible;
                    ImageGallery.Focus();
                    viewModel.CloseRequested += () => ImageGallery.Visibility = Visibility.Collapsed;

        }
    }
}
