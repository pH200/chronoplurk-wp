using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Compose
{
    [NotifyForAll]
    public class FriendsSelectionPageViewModel : Screen
    {
        private const string BopomoList = "ㄅㄆㄇㄈㄉㄊㄋㄌㄍㄎㄏㄐㄑㄒㄓㄔㄕㄖㄗㄘㄙㄚㄛㄜㄝㄞㄟㄠㄡㄢㄣㄤㄥㄦㄧㄨㄩˊˇˋ˙";

        private string _searchTextBoxDistinctKey = null;

        private IDisposable _downloadDisposable;
        private IDisposable _searchDisposable;

        protected INavigationService NavigationService { get; set; }

        protected IProgressService ProgressService { get; set; }

        protected FriendsFansCompletionService FriendsFansCompletionService { get; set; }

        public string SearchTextBox { get; set; }

        public BindableCollection<FriendResultItemViewModel> ResultItems { get; set; }

        public BindableCollection<CompletionUser> SelectedItems
        {
            get { return FriendsFansCompletionService.SelectedItems; }
        }

        public FriendsSelectionPageViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            FriendsFansCompletionService friendsFansCompletionService)
        {
            NavigationService = navigationService;
            ProgressService = progressService;
            FriendsFansCompletionService = friendsFansCompletionService;
            FriendsFansCompletionService.Load();
        }

        protected override void OnViewLoaded(object view)
        {
            if (!FriendsFansCompletionService.IsDownloaded)
            {
                ShowFirstTimeMessage();
                Download();
                ShowHelpMessage();
            }
            base.OnViewLoaded(view);
        }

        public void Search(string query)
        {
            var completion = FriendsFansCompletionService.Completion;
            Search(completion, query);
        }

        public void Search(FriendsFansCompletion completion, string query)
        {
            if (completion == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(query))
            {
                ResultItems = null;
                return;
            }
            if (_searchDisposable != null)
            {
                _searchDisposable.Dispose();
            }
            var observable = Observable.Start(() => completion.Search(query));
            _searchDisposable = observable.Subscribe(result =>
            {
                var items = from item in result
                            let selected = (SelectedItems.FirstOrDefault(u => u.Id == item.Id).Id != 0)
                            select new FriendResultItemViewModel(item, selected);
                ResultItems = new BindableCollection<FriendResultItemViewModel>(items);
            });
        }

        public void Download()
        {
            if (_downloadDisposable != null)
            {
                _downloadDisposable.Dispose();
            }
            ProgressService.Show("Downloading Friends List");
            var download = FriendsFansCompletionService.DownloadAsync();
            _downloadDisposable = download.Subscribe(
                completion => Execute.OnUIThread(() => Search(completion, SearchTextBox)),
                () => Execute.OnUIThread(ProgressService.Hide));
            download.Connect();
        }

        public void ShowFirstTimeMessage()
        {
            var message = "This is your first time using friends list." + Environment.NewLine +
                          "Friends list will download automatically.";
            MessageBox.Show(message);
        }

        public void ShowHelpMessage()
        {
            var message = "To update friends list, you have to click update button at menu bar manually";
            MessageBox.Show(message);
        }

        public void OnTextChanged()
        {
            if (SearchTextBox == _searchTextBoxDistinctKey)
            {
                return;
            }
            else
            {
                _searchTextBoxDistinctKey = SearchTextBox;
            }

            if (!(BopomoList.Any(letter => SearchTextBox.Contains(letter))))
            {
                Search(SearchTextBox);
            }
        }

        public void OnItemTap(object dataContext)
        {
            var item = dataContext as FriendResultItemViewModel;
            if (item != null)
            {
                if (SelectedItems.FirstOrDefault(u => u.Id == item.Id).Id == 0)
                {
                    item.IsSelected = true;
                    SelectedItems.Add(item.CompletionUser);
                }
            }
        }

        public void MenuDelete(object dataContext)
        {
            var item = (CompletionUser)dataContext;
            if (ResultItems != null)
            {
                var resultItem = ResultItems.FirstOrDefault(i => i.Id == item.Id);
                if (resultItem != null)
                {
                    resultItem.IsSelected = false;
                }
            }
            SelectedItems.Remove(item);
        }

        #region App Bar

        public void DownloadAppBar()
        {
            Download();
        }

        public void CompleteAppBar()
        {
            NavigationService.GoBack();
        }

        public void ClearAppBar()
        {
            if (SelectedItems != null)
            {
                SelectedItems.Clear();
            }
        }

        public void HelpAppBar()
        {
            ShowHelpMessage();
        }

        #endregion
    }

    [NotifyForAll]
    public sealed class FriendResultItemViewModel : PropertyChangedBase
    {
        public CompletionUser CompletionUser { get; set; }

        public int Id { get { return CompletionUser.Id; } }

        public string NickName { get { return CompletionUser.NickName; } }

        public string DisplayName { get { return CompletionUser.DisplayName; } }

        public string FullName { get { return CompletionUser.FullName; } }

        public bool IsSelected { get; set; }

        public SolidColorBrush SelectedBrush
        {
            get
            {
                return IsSelected
                           ? PlurkResources.PhoneAccentBrush
                           : PlurkResources.PhoneForegroundBrush;
            }
        }

        public FriendResultItemViewModel()
        {
        }

        public FriendResultItemViewModel(CompletionUser user, bool isSelected)
        {
            CompletionUser = user;
            IsSelected = isSelected;
        }
    }
}
