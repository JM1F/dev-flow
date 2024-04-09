using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace dev_flow.Commands;

/// <summary>
/// Represents an asynchronous command for the Command design pattern.
/// </summary>
public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _executeAsync;
    private bool _isExecuting;

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="executeAsync">The asynchronous action to be executed.</param>
    public AsyncRelayCommand(Func<Task> executeAsync)
    {
        _executeAsync = executeAsync;
    }


    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. Ignored in this implementation.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(object parameter)
    {
        return !_isExecuting;
    }

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">Data used by the command.</param>
    public async void Execute(object parameter)
    {
        _isExecuting = true;
        try
        {
            await _executeAsync();
        }
        finally
        {
            _isExecuting = false;
        }
    }

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}