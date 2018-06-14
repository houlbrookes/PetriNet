using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace PetriNet
{
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> execute;
        readonly Predicate<T> canExecute;

        public static List<string> Log = new List<string>();

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute == null ? T => true : canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (parameter is T theT) return canExecute(theT);
            else return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (parameter is T theT) execute(theT);
        }

        #endregion // ICommand Members
    }

}
