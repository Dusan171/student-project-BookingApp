using System.Windows;

namespace BookingApp.Utilities
{

    public static class MessageHelper
    {

        public static void ShowSuccess(string message, string title = "Uspeh")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public static void ShowError(string message, string title = "Greška")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }


        public static void ShowWarning(string message, string title = "Upozorenje")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }


        public static void ShowInfo(string message, string title = "Informacija")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public static bool ShowConfirmation(string message, string title = "Potvrda")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }


        public static MessageBoxResult ShowCustomConfirmation(string message, string title, MessageBoxButton buttons)
        {
            return MessageBox.Show(message, title, buttons, MessageBoxImage.Question);
        }


        public static void ShowValidationError(string fieldName, string errorMessage)
        {
            ShowError($"{fieldName}: {errorMessage}", "Greška u podacima");
        }


        public static void ShowValidationErrors(string[] errors)
        {
            if (errors == null || errors.Length == 0)
                return;

            string message = "Pronađene su sledeće greške:\n\n";
            for (int i = 0; i < errors.Length; i++)
            {
                message += $"• {errors[i]}\n";
            }

            ShowError(message, "Greške u podacima");
        }


        public static bool ShowLogoutConfirmation()
        {
            return ShowConfirmation("Da li ste sigurni da se želite odjaviti?", "Odjava");
        }


        public static bool ShowDeleteConfirmation(string itemName)
        {
            return ShowConfirmation($"Da li ste sigurni da želite da obrišete '{itemName}'?\n\nOva akcija se ne može poništiti.", "Potvrda brisanja");
        }


        public static MessageBoxResult ShowUnsavedChangesConfirmation()
        {
            return ShowCustomConfirmation(
                "Imate nesačuvane izmene. Da li želite da ih sačuvate pre zatvaranja?",
                "Nesačuvane izmene",
                MessageBoxButton.YesNoCancel);
        }


        public static void ShowOperationSuccess(string operationName)
        {
            ShowSuccess($"{operationName} je uspešno završeno!");
        }


        public static void ShowOperationError(string operationName, string? errorDetails = null)
        {
            string message = $"Greška prilikom izvršavanja operacije '{operationName}'.";
            if (!string.IsNullOrEmpty(errorDetails))
            {
                message += $"\n\nDetalji: {errorDetails}";
            }
            ShowError(message);
        }


        public static void ShowNoResults(string searchType = "rezultata")
        {
            ShowInfo($"Nema pronađenih {searchType} za zadate kriterijume.", "Nema rezultata");
        }


        public static void ShowFeatureNotImplemented(string featureName)
        {
            ShowInfo($"Funkcionalnost '{featureName}' će biti implementirana u budućoj verziji.", "U razvoju");
        }
    }
}