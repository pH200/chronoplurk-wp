using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Compose
{
    [ImplementPropertyChanged]
    public class FriendsSelectionPageViewModel : Screen
    {
        private const string BopomoList = "ㄅㄆㄇㄈㄉㄊㄋㄌㄍㄎㄏㄐㄑㄒㄓㄔㄕㄖㄗㄘㄙㄚㄛㄜㄝㄞㄟㄠㄡㄢㄣㄤㄥㄦㄧㄨㄩˊˇˋ˙";

        private string _searchTextBoxDistinctKey = null;

        private IDisposable _downloadDisposable;
        private IDisposable _searchDisposable;

        protected INavigationService NavigationService { get; set; }

        protected IProgressService ProgressService { get; set; }

        protected IPlurkService PlurkService { get; set; }

        protected FriendsFansCompletionService FriendsFansCompletionService { get; set; }

        public string SearchTextBox { get; set; }

        public BindableCollection<FriendResultItemViewModel> ResultItems { get; set; }

        public BindableCollection<CompletionUser> SelectedItems
        {
            get { return FriendsFansCompletionService.SelectedItems; }
        }
        
        private BindableCollection<FriendResultItemViewModel> AllItems { get; set; }

        public FriendsSelectionPageViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            FriendsFansCompletionService friendsFansCompletionService)
        {
            NavigationService = navigationService;
            ProgressService = progressService;
            PlurkService = plurkService;
            FriendsFansCompletionService = friendsFansCompletionService;
            FriendsFansCompletionService.Load();
        }

        protected override void OnActivate()
        {
            if (FriendsFansCompletionService.LoadedUserId != 0)
            {
                if (PlurkService.UserId != FriendsFansCompletionService.LoadedUserId)
                {
                    Download();
                }
                else if (AllItems == null)
                {
                    InitializeAllItems(FriendsFansCompletionService.Completion);
                }
            }
            base.OnActivate();
        }

        protected override void OnViewLoaded(object view)
        {
            if (!FriendsFansCompletionService.IsDownloaded)
            {
                ShowFirstTimeMessage();
                Download();
                ShowHelpMessage();
            }
            else
            {
                Search(SearchTextBox);
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
                ResultItems = AllItems;
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
            var download = FriendsFansCompletionService.DownloadAsync();
            _downloadDisposable = download
                .DoProgress(ProgressService, AppResources.msgDownloadingFriendsList)
                .Subscribe(completion =>
                {
                    InitializeAllItems(completion);
                    Execute.OnUIThread(() => Search(completion, SearchTextBox));
                });
            download.Connect();
        }

        private void InitializeAllItems(FriendsFansCompletion completion)
        {
            if (completion != null)
            {
                var items = from item in completion.Lookup.Values
                            let selected = (SelectedItems.FirstOrDefault(u => u.Id == item.Id).Id != 0)
                            select new FriendResultItemViewModel(item, selected);
                AllItems = new BindableCollection<FriendResultItemViewModel>(items);
            }
        }

        public void ShowFirstTimeMessage()
        {
            var message = AppResources.friendsListFirst.Replace("\\n", Environment.NewLine);
            MessageBox.Show(message);
        }

        public void ShowHelpMessage()
        {
            var message = AppResources.friendsListHelp.Replace("\\n", Environment.NewLine);
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
                var selectedItem = SelectedItems.FirstOrDefault(u => u.Id == item.Id);
                if (selectedItem.Id == default(int)) // struct default
                {
                    item.IsSelected = true;
                    SelectedItems.Add(item.CompletionUser);
                }
                else
                {
                    item.IsSelected = false;
                    SelectedItems.Remove(selectedItem);
                }
            }
        }

        public void OnSelectedItemTap(object dataContext)
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
                if (ResultItems != null)
                {
                    foreach (var item in ResultItems)
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }

        public void HelpAppBar()
        {
            ShowHelpMessage();
        }

        #endregion
    }

    [ImplementPropertyChanged]
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
