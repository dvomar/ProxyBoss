using System;
using System.Windows;
using System.Windows.Input;

namespace ProxyBoss.Commands
{
    public class CloseThisWindowCommand : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter) =>
            //we can only close Windows
            parameter is Window;

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                ((Window)parameter).Close();
        }

        #endregion

        private CloseThisWindowCommand() { }

        public static readonly ICommand Instance = new CloseThisWindowCommand();
    }
}