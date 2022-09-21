public struct ObjectCreatedSignal<T>
{
    public ObjectCreatedSignal(T obj){
        if(obj == null)
            throw new System.ArgumentNullException();

        Object = obj;
    }

    public T Object { get; }
}