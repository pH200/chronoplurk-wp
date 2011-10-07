using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Helpers;

namespace ChronoPlurk.ViewModels
{
    public sealed class SearchRecordsViewModel : Screen, IChildT<ISearchPage>
    {
        public const int MaxRecords = 20;

        public ObservableCollection<SearchRecord> Items { get; set; }

        private int _listSelectedIndex = -1; // Must defualt as -1

        public int ListSelectedIndex
        {
            get { return _listSelectedIndex; }
            set
            {
                if (_listSelectedIndex == value) return;
                _listSelectedIndex = value;
                NotifyOfPropertyChange(() => ListSelectedIndex);
            }
        }

        public SearchRecordsViewModel()
        {
            Items = new ObservableCollection<SearchRecord>();

            this.DisplayName = "records";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            string[] records;
            if (IsoSettings.TryGetValue(SearchRecord.StorageKey, out records))
            {
                foreach (var t in records.Where(t => !Items.Any(r => r.Query == t)))
                {
                    Items.Add(new SearchRecord(t));
                }
            }
        }

        public void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (ListSelectedIndex == -1)
            {
                return;
            }
            
            var item = Items[ListSelectedIndex];
            var parent = this.GetParent();
            if (parent != null)
            {
                parent.Search(item.Query);
            }

            ListSelectedIndex = -1;
        }

        public void Add(string record)
        {
            Items.Remove(Items.FirstOrDefault(r => r.Query == record));
            Items.Insert(0, new SearchRecord(record));
            if (Items.Count > MaxRecords)
            {
                Items.RemoveAt(Items.Count - 1);
            }

            SaveRecords();
        }

        private void SaveRecords()
        {
            IsoSettings.AddOrChange(SearchRecord.StorageKey, Items.Select(s => s.Query).ToArray());
        }
    }
    public class SearchRecord
    {
        public const string StorageKey = "PlurkSearchRecords";

        public string Query { get; set; }
        public SearchRecord(string query)
        {
            Query = query;
        }
    }
}
