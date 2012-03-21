using System;
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
                var index = this.Count;
                foreach (var item in items)
                {
                    InsertItemBase(index, item);
                    index++;
                }
            });
        }
    }
}
