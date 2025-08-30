using System;
using System.Windows.Input;

namespace BookingApp.Utilities
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Predicate<T?>? _canExecute;

        public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null) return true;
            if (parameter is T t) return _canExecute(t);
            if (parameter == null && typeof(T).IsClass) return _canExecute(default(T));
            return false;
        }

        public void Execute(object? parameter)
        {
            if (parameter is T t)
                _execute(t);
            else if (parameter == null && typeof(T).IsClass)
                _execute(default(T));
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}