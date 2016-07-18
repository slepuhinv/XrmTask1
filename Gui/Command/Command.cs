using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gui
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        Action _TargetExecuteMethod;
        Func<bool> _TargetCanExecuteMethod;

        public Command(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public Command(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_TargetCanExecuteMethod != null)
                return _TargetCanExecuteMethod();
            if (_TargetExecuteMethod != null)
                return true;
            return false;
        }

        public void Execute(object parameter)
        {
            _TargetExecuteMethod?.Invoke();
        }
    }
}
