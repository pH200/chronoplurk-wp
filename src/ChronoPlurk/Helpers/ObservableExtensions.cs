using System;

namespace ChronoPlurk.Helpers
{
    public static class ObservableExtensions
    {
        public static void SubscribeAndForget<T>(this IObservable<T> observable)
        {
            IDisposable sub = null;
            sub = observable.Subscribe(t =>
            {
            }, err =>
            {
                if (sub != null)
                {
                    sub.Dispose();
                }
            }, () =>
            {
                if (sub != null)
                {
                    sub.Dispose();
                }
            });
        }
    }
}
