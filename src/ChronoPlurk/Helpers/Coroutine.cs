using System;
using System.Reactive.Linq;
using System.Threading;

namespace ChronoPlurk.Helpers
{
    public static class ThreadEx
    {
        public static void OnUIThread(this Action action)
        {
            Caliburn.Micro.Execute.OnUIThread(action);
        }

        public static void OnThreadPool(this Action action)
        {
            ThreadPool.QueueUserWorkItem(state => action());
        }

        public static void OnThreadPool(this Action action, Action onCompleteUI)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                action();
                onCompleteUI.OnUIThread();
            });
        }

        public static void OnThreadPool<T>(this Func<T> action, Action<T> onCompleteUI)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                var result = action();
                OnUIThread(() => onCompleteUI(result));
            });
        }

        public static void OnThreadPoolIgnoreExceptions(this Action action)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    action();
                }
                catch (Exception)
                {
                    return;
                }
            });
        }

        public static void TimerAction(TimeSpan dueTime, Action action)
        {
            Observable.Timer(dueTime).Subscribe(tick => { }, action);
        }

        public static void TimerActionUI(TimeSpan dueTime, Action action)
        {
            Observable.Timer(dueTime).ObserveOnDispatcher().Subscribe(tick => { }, action);
        }
    }
}
