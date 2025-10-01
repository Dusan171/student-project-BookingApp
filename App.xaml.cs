using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using BookingApp.Services;

namespace BookingApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            base.OnStartup(e);

            LocalizationService.Instance.SetLanguage("en-US");

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = $"An unhandled exception occurred: \n\n" +
                                  $"Message: {e.Exception.Message}\n\n" +
                                  $"Stack Trace: \n{e.Exception.StackTrace}";

            MessageBox.Show(errorMessage, "Critical Application Error", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;

            Application.Current.Shutdown();
        }
    }
}
