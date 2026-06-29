namespace Cygnus.Models;

/// <summary>
/// For models that each want to notify multiple observers - without holding a reference to them
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ObservableModel<T> where T : class
{
    private static readonly object s_notificationLock = new();
    private List<WeakReference<T>> _observers = [];

    public void RemoveAllObservers() { _observers = []; }
    
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
