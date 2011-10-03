namespace ChronoPlurk.ViewModels
{
    public interface IRefreshSync
    {
        void RefreshSync();
        bool RefreshOnActivate { get; set; }
    }
}
