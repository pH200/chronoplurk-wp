using System;
using System.Reactive.Linq;

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
                return Observable.Throw<TSource>(e);
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
