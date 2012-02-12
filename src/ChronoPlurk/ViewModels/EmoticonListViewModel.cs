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
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class EmoticonListViewModel : Screen
    {
        public BindableCollection<KeyValuePair<string, string>> Items { get; set; }

        public EmoticonListViewModel()
        {
            Items = new BindableCollection<KeyValuePair<string, string>>();
        }

        public EmoticonListViewModel(IEnumerable<KeyValuePair<string, string>> collection)
        {
            if (collection != null)
            {
                Items = new BindableCollection<KeyValuePair<string, string>>(collection);
            }
            else
            {
                Items = new BindableCollection<KeyValuePair<string, string>>();
            }
        }
    }
}
