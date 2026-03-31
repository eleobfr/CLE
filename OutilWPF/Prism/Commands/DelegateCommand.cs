using System;
using System.Windows.Input;

namespace Prism.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action executeMethod;
        private readonly Func<bool> canExecuteMethod;

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod = null)
        {
            this.executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
            this.canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecuteMethod?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            executeMethod();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> executeMethod;
        private readonly Func<T, bool> canExecuteMethod;

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod = null)
        {
            this.executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
            this.canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecuteMethod?.Invoke(ConvertParameter(parameter)) ?? true;
        }

        public void Execute(object parameter)
        {
            executeMethod(ConvertParameter(parameter));
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private static T ConvertParameter(object parameter)
        {
            if (parameter == null)
                return default;

            if (parameter is T typed)
                return typed;

            return (T)parameter;
        }
    }
}
