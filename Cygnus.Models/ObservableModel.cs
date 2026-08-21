namespace Cygnus.Models;

/// <summary>
/// For models that each want to notify multiple observers - without holding a reference to them
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ObservableModel<T> where T : class
{
    private static readonly object s_observerLock = new();
    private List<WeakReference<T>> _observers = [];
    private int _observerCount;

    protected bool IsBeingObserved
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnIsBeingObservedChanged(value);
            }
        }
    }

    public void RemoveAllObservers() 
    {
        lock (s_observerLock)
        {
            _observers = [];
        }

        SetObserverCount(0);
    }

    public void AddObservers(IEnumerable<T> registeredObservers)
    {
        foreach (T observer in registeredObservers)
        {
            AddObserver(observer);
        }
    }

    public virtual void AddObserver(T observer)
    {
        int newCount = 0;
        lock (s_observerLock)
        {
            _observers.Add(new WeakReference<T>(observer));
            newCount = _observers.Count;
        }

        SetObserverCount(newCount);
    }

    /// <summary>
    /// Removes the observer from the list of observers. If the observer is not found, nothing happens.
    /// </summary>
    /// <param name="observer"></param>
    /// <returns>The number of remaining observers.</returns>
    public virtual void RemoveObserver(T observer)
    {
        int newCount = 0;
        lock (s_observerLock)
        {
            for (int i = 0; i < _observers.Count; i++)
            {
                if (_observers[i].TryGetTarget(out T? target) && target == observer)
                {
                    _observers.RemoveAt(i);
                }
            }

            newCount = _observers.Count;
        }

        SetObserverCount(newCount);
    }

    public void NotifyObservers(Action<T> action)
    {
        NotifyObservers(action, _observers);
    }

    protected void NotifyObservers<A>(Action<A> action, List<WeakReference<A>> observers) where A : class
    {
        List<A> snapshot = [];
        lock (s_observerLock)
        {
            _observers.RemoveAll(wr => !wr.TryGetTarget(out _));

            SetObserverCount(observers.Count);

            foreach (var reference in observers)
            {
                if (reference.TryGetTarget(out A? observer))
                {
                    snapshot.Add(observer);
                }
            }
        }

        // It is now safe to iterate outside of the lock
        foreach (var observer in snapshot)
        {
            action(observer);
        }
    }

    private void SetObserverCount(int newCount)
    {
        Interlocked.Exchange(ref _observerCount, newCount);
        IsBeingObserved = newCount > 0;
    }

    /// <summary>
    // Override in derived classes to handle observation changes
    /// </summary>
    /// <param name="count"></param>
    protected virtual void OnIsBeingObservedChanged(bool isBeingObserved) { }
}
