using System.Windows;
using System.Windows.Controls;

namespace BookingApp.Presentation.View.Guide.Controls
{
    public partial class PdfModalControl : UserControl
    {
        public PdfModalControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler ClosePDFOverlayClick;
        public event RoutedEventHandler OpenPDFExternalClick;

        private void ClosePDFOverlay_Click(object sender, RoutedEventArgs e)
        {
            ClosePDFOverlayClick?.Invoke(sender, e);
        }

        private void OpenPDFExternal_Click(object sender, RoutedEventArgs e)
        {
            OpenPDFExternalClick?.Invoke(sender, e);
        }

        public WebBrowser WebBrowser => PDFWebBrowser;
        public Border Overlay => PDFOverlay;
    }
}