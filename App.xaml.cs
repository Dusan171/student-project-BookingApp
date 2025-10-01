using System.Windows;
using System.Text;

namespace BookingApp
{
    public partial class App : Application
    {
        public App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            InitializeComponent();
        }
    }
}