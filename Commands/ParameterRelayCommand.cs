using System;
using System.Windows.Input;

namespace dev_flow.Commands;

/// <summary>
/// Represents a command that can be executed with a parameter of type T.
/// </summary>
/// <typeparam name="T">The type of the command parameter.</typeparam>
public class ParameterRelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Predicate<T> _canExecute;

    /// <summary>
    /// Initialises a new instance of the <see cref="ParameterRelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">The action to be executed.</param>
    /// <param name="canExecute">The predicate to determine if the command can be executed. Optional.</param>
    public ParameterRelayCommand(Action<T> execute, Predicate<T> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute((T)parameter);
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    public void Execute(object parameter)
    {
        _execute((T)parameter);
    }

    public event EventHandler CanExecuteChanged;

    protected virtual void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}