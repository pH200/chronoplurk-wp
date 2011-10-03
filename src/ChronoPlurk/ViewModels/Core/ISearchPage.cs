namespace ChronoPlurk.ViewModels
{
    public interface ISearchPage
    {
        string SearchField { get; set; }
        void Search(string query);
    }
}