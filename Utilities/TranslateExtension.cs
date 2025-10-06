using System;
using System.Windows.Data;
using System.Windows.Markup;
using BookingApp.Services;

namespace BookingApp.Utilities
{
    public class TranslateExtension : MarkupExtension
    {
        public string Key { get; set; }

        public TranslateExtension(string key)
        {
            Key = key;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding($"[{Key}]")
            {
                Source = LocalizationService.Instance,
                Mode = BindingMode.OneWay
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}