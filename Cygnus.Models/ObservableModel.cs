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
        lock (s_notificationLock)
        {
            for (int i = observers.Count - 1; i >= 0; i--)
            {
                WeakReference<A> observerRef = observers[i];
                if (observerRef.TryGetTarget(out A? observer) == true)
                {
                    action(observer);
                }
                else
                {
                    observers.RemoveAt(i);
                }
            }
        }
    }
}
