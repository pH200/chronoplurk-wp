using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using ChronoPlurk.Core;
using PropertyChanged;

namespace ChronoPlurk.ViewModels
{
    [ImplementPropertyChanged]
    public class EmoticonListViewModel : Screen
    {
        public IList<KeyValuePair<string, string>> Items { get; set; }

        private EmoticonListViewModel()
        {
        }

        public EmoticonListViewModel(IEnumerable<KeyValuePair<string, string>> collection)
        {
            if (collection != null)
            {
                Items = new List<KeyValuePair<string, string>>(collection);
            }
            else
            {
                Items = new List<KeyValuePair<string, string>>();
            }
        }

        public static EmoticonListViewModel CreateBindable(IEnumerable<KeyValuePair<string, string>> collection)
        {
            var items = new BindableCollection<KeyValuePair<string, string>>();
            if (collection != null)
            {
                items.AddRange(collection);
            }
            return new EmoticonListViewModel() { Items = items };
        }
    }
}
