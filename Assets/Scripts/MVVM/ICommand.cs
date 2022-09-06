using System;

public interface ICommand
{
    event Action CanExecuteChanged;

    bool CanExecute();
    void Execute();
}

public interface ICommand<T>
{
    event Action CanExecuteChanged;

    bool CanExecute(T parameter);
    void Execute(T parameter);
}
