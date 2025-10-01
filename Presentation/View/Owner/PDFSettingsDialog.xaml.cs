using System;
using System.Windows;
using BookingApp.Services;

namespace BookingApp.Presentation.View.Owner
{
    public partial class PDFSettingsDialog : Window
    {
        public PDFSettingsDialog()
        {
            InitializeComponent();
            this.DataContext = Injector.CreatePDFSettingsViewModel(Close);
        }
    }
}