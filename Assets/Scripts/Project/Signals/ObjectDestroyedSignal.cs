public struct ObjectDestroyedSignal<T>
{
    public ObjectDestroyedSignal(T obj){
        if(obj == null)
            throw new System.ArgumentNullException();

        Object = obj;
    }

    public T Object { get; set; }
}