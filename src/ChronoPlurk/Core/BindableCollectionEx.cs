using System.Collections.Generic;
using Caliburn.Micro;

namespace ChronoPlurk.Core
{
    public class AdditiveBindableCollection<T> : BindableCollection<T>
    {
        public AdditiveBindableCollection()
        {
        }

        public AdditiveBindableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

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
