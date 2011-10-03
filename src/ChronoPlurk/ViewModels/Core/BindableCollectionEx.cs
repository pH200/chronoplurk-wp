using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Caliburn.Micro;

namespace ChronoPlurk.ViewModels
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
