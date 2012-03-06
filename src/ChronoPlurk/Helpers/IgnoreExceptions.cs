using System;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;

namespace ChronoPlurk.Helpers
{
    public static class IgnoreExceptionsExtensions
    {
        public static IObservable<TSource> Ignore<TSource, TException>(this IObservable<TSource> source)
            where TException : Exception
        {
            return source.Catch<TSource, TException>(e =>
            {
#if DEBUG
                Execute.OnUIThread(() => MessageBox.Show(
                    source.ToString() + Environment.NewLine + e.ToString(),
                    "DEBUG",
                    MessageBoxButton.OK));
                return Observable.Empty<TSource>();
#else
                return Observable.Empty<TSource>();
#endif
            });
        }

        public static IObservable<TSource> IgnoreAllExceptions<TSource>(this IObservable<TSource> source)
        {
            return source.Ignore<TSource, Exception>();
        }
    }
}
