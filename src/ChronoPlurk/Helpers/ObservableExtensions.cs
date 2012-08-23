using System;
using System.Reactive.Linq;
using ChronoPlurk.Services;

namespace ChronoPlurk.Helpers
{
    public static class ObservableExtensions
    {
        public static IObservable<T> DoProgress<T>(this IObservable<T> observable, IProgressService progressService, string message = null)
        {
            var id = progressService.Show(message);
            return observable.DoProgress(progressService, id);
        }

        public static IObservable<T> DoProgress<T>(this IObservable<T> observable, IProgressService progressService, int id)
        {
            return observable.Do(o => progressService.Hide(id)).Catch<T, Exception>(exception =>
            {
                progressService.Hide(id);
                return Observable.Throw<T>(exception);
            });
        }

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
