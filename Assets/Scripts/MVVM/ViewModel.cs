public abstract class ViewModel
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
    
    public interface IObservableProperty<T>{
        event System.Action Changed;
        T Value { get; set; }
    }
}
