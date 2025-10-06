using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using BookingApp.Services;
using PdfSharp.Fonts;

namespace BookingApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Registracija encoding-a za CSV i PDF
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Registracija fontova za PdfSharp
            GlobalFontSettings.FontResolver = new AppFontResolver();

            base.OnStartup(e);

            // Podešavanje jezika aplikacije
            LocalizationService.Instance.SetLanguage("en-US");

            // Handler za neobrađene exception-e
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = $"An unhandled exception occurred: \n\n" +
                                  $"Message: {e.Exception.Message}\n\n" +
                                  $"Stack Trace: \n{e.Exception.StackTrace}";

            MessageBox.Show(errorMessage, "Critical Application Error",
                          MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}