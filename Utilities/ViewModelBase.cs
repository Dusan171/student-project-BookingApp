using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookingApp.Utilities // Proverite da li je namespace ispravan
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Poziva se kada se vrednost svojstva promeni, da bi se obavestio UI.
        /// [CallerMemberName] automatski popunjava ime svojstva koje je pozvalo metodu.
        /// </summary>
        /// <param name="propertyName">Ime svojstva koje se promenilo.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}