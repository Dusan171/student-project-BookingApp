using System;
using System.Windows;
using BookingApp.Domain.Model;
using BookingApp.Presentation.ViewModel.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class ComplexTourAcceptWindow : Window
    {
        public ComplexTourAcceptViewModel ViewModel { get; private set; }

        public ComplexTourAcceptWindow(ComplexTourRequestPart part)
        {
            InitializeComponent();

            ViewModel = new ComplexTourAcceptViewModel(part);
            DataContext = ViewModel;

            ViewModel.RequestClose += (sender, e) =>
            {
                DialogResult = ViewModel.DialogResult;
                Close();
            };
        }
    }
}