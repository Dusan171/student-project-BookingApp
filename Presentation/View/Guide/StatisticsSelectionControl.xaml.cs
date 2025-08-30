using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BookingApp.Domain.Model; 
using BookingApp.Repositories;
using BookingApp.Presentation.View.Guide;

namespace BookingApp.Presentation.View.Guide
{
    public partial class StatisticsSelectionControl : UserControl
    {
        MainPage mainPage; 
        public StatisticsSelectionControl(MainPage main)
        {
            InitializeComponent();
            mainPage = main;
        }
        private void ShowTourStatistics_Click(object sender, RoutedEventArgs e)
        {
            //mainPage.ContentFrame.Content = new TourStatisticsControl(mainPage);
            mainPage.ContentFrame.Navigate(new Page { Content = new TourStatisticsControl(mainPage) });
        }
        private void ShowRequestStatistics_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
