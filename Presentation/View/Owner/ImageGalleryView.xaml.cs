using BookingApp.Presentation.ViewModel.Owner;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BookingApp.Presentation.Owner
{
   
    public partial class ImageGalleryView : UserControl
    {
        


        public ImageGalleryView()
        {
            InitializeComponent();
            this.Focus();
        }

 
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is ImageGalleryViewModel vm)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        vm.PrevCommand.Execute(null);
                        break;
                    case Key.Right:
                        vm.NextCommand.Execute(null);
                        break;
                    case Key.Escape:
                        vm.CloseCommand.Execute(null);
                        break;
                }
            }
        }
    }
}
