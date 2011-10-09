using System.Collections.Generic;
using Caliburn.Micro;

namespace ChronoPlurk.Core
{
    public class AdditiveBindableCollection<T> : BindableCollection<T>
    {
        public override void AddRange(IEnumerable<T> items)
        {
            Execute.OnUIThread(() =>
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            });
        }
    }
}
