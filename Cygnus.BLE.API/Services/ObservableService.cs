namespace Cygnus.BLE.API.Services;

/// <summary>
/// For services that each want to notify multiple observers - without holding a reference to them
/// </summary>
/// <typeparam name="T"></typeparam>
internal abstract class ObservableService<T> where T : class
{
    private static readonly object s_notificationLock = new();
    private readonly List<WeakReference<T>> _observers = [];

    public void AddObservers(IEnumerable<T> registeredObservers)
    {
        foreach (T observer in registeredObservers)
        {
            AddObserver(observer);
        }
    }

    public virtual void AddObserver(T observer) => _observers.Add(new WeakReference<T>(observer));

    public void NotifyObservers(Action<T> action)
    {
        List<WeakReference<T>> observers = _observers;
        NotifyObservers(action, observers);
    }

    protected static void NotifyObservers<A>(Action<A> action, List<WeakReference<A>> observers) where A : class
    {
        List<WeakReference<A>> expiredObservers = [];
        lock (s_notificationLock)
        {
            foreach (WeakReference<A> observerRef in observers)
            {
                if (observerRef.TryGetTarget(out A? observer) == true)
                {
                    action(observer);
                }
                else
                {
                    expiredObservers.Add(observerRef);
                }
            }
            foreach (WeakReference<A> observer in expiredObservers)
            {
                observers.Remove(observer);
            }
        }
    }
}

/// <summary>
/// For services that contain multiple indexed items that want to notify multiple observers - without holding a reference to them
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="I"></typeparam>
internal abstract class IndexedObservableService<I, T, S> : ObservableService<S>
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
