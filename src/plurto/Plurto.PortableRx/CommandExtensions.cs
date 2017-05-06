using Plurto.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plurto.Commands
{
    public static class CommandExtensions
    {
        public static IObservable<T> ToObservable<T>(this PlurkCommand<T> command)
        {
            // Disable explicit throw and handle error by ThrowWeb for performance.
            return Observable.FromAsync(command.LoadWebResultAsync).ThrowWeb().Select(result => result.Result);
        }

        private static IObservable<WebResult<T>> ThrowWeb<T>(this IObservable<WebResult<T>> observable)
        {
            return observable.SelectMany(result =>
            {
                if (result.Error != null)
                {
                    return Observable.Throw(result.Error, result);
                }
                return Observable.Return(result);
            });
        }
    }
}
