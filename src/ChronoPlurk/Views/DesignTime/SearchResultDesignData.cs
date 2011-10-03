using System;
using Caliburn.Micro;
using ChronoPlurk.ViewModels;

namespace ChronoPlurk.Views.DesignTime
{
    public class SearchResultDesignData
    {
        public BindableCollection<PlurkItemViewModel> Items { get; set; }
        public SearchResultDesignData()
        {
            Items = new BindableCollection<PlurkItemViewModel>()
            {
                new PlurkItemViewModel(){Username = "Design1", PostTimeFromNow = new TimeSpan(100), ContentRaw = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum"},
                new PlurkItemViewModel(){Username = "Design2", PostTimeFromNow = new TimeSpan(100), Qualifier = "like", ContentRaw = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum"},
            };
        }
    }
}
