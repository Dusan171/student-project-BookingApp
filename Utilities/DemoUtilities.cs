using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookingApp.Utilities
{
    public static class DemoUtilities
    {
        
        public static T FindControl<T>(DependencyObject parent, string controlName) where T : FrameworkElement
        {
            if (parent == null) return null;

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is T control && control.Name == controlName)
                {
                    return control;
                }

                var result = FindControl<T>(child, controlName);
                if (result != null) return result;
            }

            return null;
        }

        
        public static void SimulateTypingDelay(Action action, int delayMs = 500)
        {
            System.Threading.Tasks.Task.Delay(delayMs).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.BeginInvoke(action);
            });
        }
    }
}

