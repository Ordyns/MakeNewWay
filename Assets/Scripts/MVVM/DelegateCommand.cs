using System;

public class DelegateCommand<T> : ICommand<T>
{
    private readonly Action<T> _executeAction;
    private readonly Func<T, bool> _canExecuteFunc;

    public DelegateCommand(Action<T> executeAction, Func<T, bool> canExecuteFunc){
        _executeAction = executeAction;
        _canExecuteFunc = canExecuteFunc;
    }

    public DelegateCommand(Action<T> executeAction) : this(executeAction, (p) => true) { }

    public void Execute(T parameter) => _executeAction(parameter);

    public bool CanExecute(T parameter) => _canExecuteFunc?.Invoke(parameter) ?? true;

    public event Action CanExecuteChanged;

    public void InvokeCanExecuteChanged() => CanExecuteChanged?.Invoke();
}

public class DelegateCommand : ICommand
{
    private readonly Action _executeAction;
    private readonly Func<bool> _canExecuteFunc;

    public DelegateCommand(Action executeAction, Func<bool> canExecuteFunc){
        _executeAction = executeAction;
        _canExecuteFunc = canExecuteFunc;
    }

    public DelegateCommand(Action executeAction) : this(executeAction, () => true) { }

    public void Execute() => _executeAction();

    public bool CanExecute() => _canExecuteFunc?.Invoke() ?? true;

    public event Action CanExecuteChanged;

    public void InvokeCanExecuteChanged() => CanExecuteChanged?.Invoke();
}