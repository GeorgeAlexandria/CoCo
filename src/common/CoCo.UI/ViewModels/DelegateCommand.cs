using System;
using System.Windows.Input;

namespace CoCo.UI.ViewModels
{
    /// <summary>
    /// Custom non generic implementation of <see cref="ICommand"/>
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;
        private bool _isExecute;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) =>
            !_isExecute && (_canExecute == null || _canExecute());

        public void Execute(object parameter)
        {
            _isExecute = true;
            try
            {
                RaiseCanExecuteChanged();
                _execute();
            }
            finally
            {
                _isExecute = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            // TODO: temporary assume that this implementation isn't obliged to continue execution in the original thread.
            // If it will, use a SynchronizationContext to send|post message to a original thread
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Custom generic implementation of <see cref="ICommand"/>
    /// </summary>
    public class DelegateCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;
        private bool _isExecute;

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) =>
            !_isExecute && (_canExecute == null || parameter is T castedArgument && _canExecute(castedArgument));

        public void Execute(object parameter)
        {
            _isExecute = true;
            try
            {
                var argument = parameter is T ? (T)parameter : default(T);
                RaiseCanExecuteChanged();
                _execute(argument);
            }
            finally
            {
                _isExecute = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            // TODO: temporary assume that this implementation isn't obliged to continue execution in the original thread.
            // If it will, use a SynchronizationContext to send|post message to a original thread
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}