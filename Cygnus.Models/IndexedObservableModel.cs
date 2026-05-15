namespace Cygnus.BLE.API.Services;

/// <summary>
/// For models that contain multiple indexed items that want to notify multiple observers - without holding a reference to them
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="I"></typeparam>
public abstract class IndexedObservableModel<I, T, S> : ObservableModel<S>
    where I : struct
    where T : class
    where S : class
{
    private readonly Dictionary<I, List<WeakReference<T>>> _indexedObservers = [];

    public virtual void AddObserver(I index, T observer)
    {
        if (!_indexedObservers.TryGetValue(index, out List<WeakReference<T>>? observers))
        {
            observers = _indexedObservers[index] = [];
        }

        observers.Add(new WeakReference<T>(observer));
    }

    protected void NotifyObservers(I index, Action<T> action)
    {
        if (_indexedObservers.TryGetValue(index, out List<WeakReference<T>>? observers))
        {
            NotifyObservers(action, observers);
        }
    }
}
