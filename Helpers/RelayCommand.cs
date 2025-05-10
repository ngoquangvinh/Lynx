using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LynxUI_Main.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?>? _executeWithParam;
        private readonly Action? _executeNoParam;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _executeNoParam = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object?> execute, Func<bool>? canExecute = null)
        {
            _executeWithParam = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter)
        {
            if (_executeNoParam != null)
                _executeNoParam();
            else
                _executeWithParam?.Invoke(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value!;
            remove => CommandManager.RequerySuggested -= value!;
        }
    }


}
