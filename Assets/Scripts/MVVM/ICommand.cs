using System;

public interface ICommand
{
    event Action CanExecuteChanged;
    void InvokeCanExecuteChanged();

    bool CanExecute();
    void Execute();
}

public interface ICommand<T>
{
    event Action CanExecuteChanged;
    void InvokeCanExecuteChanged();

    bool CanExecute(T parameter);
    void Execute(T parameter);
}
