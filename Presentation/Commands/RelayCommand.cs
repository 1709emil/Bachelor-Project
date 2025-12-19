using System.Windows.Input;

namespace VisualWorkflowBuilder.UiImplementation.Commands;

public class RelayCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    private Action<object> execute { get; set; }
    private Predicate<object> canExecute { get; set; }

    public RelayCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
    {
        this.execute = executeMethod;
        this.canExecute = canExecuteMethod;
    }


    public bool CanExecute(object? parameter)
    {
        return canExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        execute(parameter);
    }


}