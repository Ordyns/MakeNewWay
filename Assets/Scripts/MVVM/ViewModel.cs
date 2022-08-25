using System;

public abstract class ViewModel : UnityEngine.MonoBehaviour
{
    public class ObservableProperty<T> : IObservableProperty<T>
    {
        public event System.Action Changed;
        private T _value;

        public ObservableProperty(T value) { Value = value; }
        public ObservableProperty() { Value = default; }

        public T Value { 
            get => _value;
            set { _value = value; Changed?.Invoke(); }
        }

        public override string ToString() => Value.ToString();
        public static implicit operator T(ObservableProperty<T> value) => value.Value;
    }

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

    public interface ICommand<T>
    {
        event Action CanExecuteChanged;

        bool CanExecute(T parameter);
        void Execute(T parameter);
    }

    public interface IObservableProperty<T>{
        event System.Action Changed;
        T Value { get; set; }
    }
}
