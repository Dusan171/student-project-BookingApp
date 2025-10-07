using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BookingApp.Domain;
using BookingApp.Repositories;
using System.Collections.ObjectModel;
using System.IO;
using BookingApp.Domain.Model;
using BookingApp.Utilities;
using BookingApp.Presentation.ViewModel.Guide;


namespace BookingApp.Presentation.View.Guide
{
    public partial class CreateTourControl : Page
    {
        public event EventHandler? Cancelled;
        public event EventHandler? TourCreated;

        private readonly CreateTourControlViewModel _viewModel;
        public Tour suggested { get; set; }

        public CreateTourControl(Tour suggestion = null)
        {
            InitializeComponent();
            suggested = suggestion ?? new Tour();
            _viewModel = new CreateTourControlViewModel(suggested);
            _viewModel.RequestClose += ViewModel_RequestClose;
            _viewModel.RequestCancel += ViewModel_RequestCancel;

            this.DataContext = _viewModel;
        }

        private void ViewModel_RequestCancel(object? sender, EventArgs e)
        {
            
            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void ViewModel_RequestClose(object? sender, EventArgs e)
        {
            TourCreated?.Invoke(this, EventArgs.Empty);
           if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }

}



