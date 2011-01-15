using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using MetroPlurk.Helpers;

namespace MetroPlurk.ViewModels
{
    public class SearchRecordsViewModel : Screen
    {
        public const int MaxRecords = 20;

        public ObservableCollection<SearchRecord> Items { get; set; }

        private ISearchPage SearchParent { get { return Parent as ISearchPage; } }

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
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var query = e.AddedItems.OfType<SearchRecord>().FirstOrDefault().Query;
                if (SearchParent != null)
                {
                    SearchParent.Search(query);
                }
            }
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
