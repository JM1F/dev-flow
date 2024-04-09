using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace dev_flow.Commands
{
    using System;
    using System.Windows.Input;

    namespace dev_flow.Commands
    {
        /// <summary>
        /// Represents a command that can be executed without a parameter.
        /// </summary>
        public class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            /// <summary>
            /// Initialises a new instance of the <see cref="RelayCommand"/> class.
            /// </summary>
            /// <param name="execute">The action to be executed.</param>
            /// <param name="canExecute">The predicate to determine if the command can be executed. Optional.</param>
            public RelayCommand(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            /// <summary>
            /// Determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">Data used by the command.</param>
            /// <returns>true if this command can be executed; otherwise, false.</returns>
            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute();
            }

            /// <summary>
            /// Executes the command.
            /// </summary>
            /// <param name="parameter">Data used by the command.</param>
            public void Execute(object parameter)
            {
                _execute();
            }

            public event EventHandler? CanExecuteChanged;

            protected virtual void OnCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}