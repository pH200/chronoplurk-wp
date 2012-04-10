using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using ChronoPlurk.Views.Compose;
using NotifyPropertyWeaver;
using Plurto.Commands;
using Plurto.Core;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Compose
{
    [NotifyForAll]
    public class ComposePageViewModel : ComposeViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly FriendsFansCompletionService _friendsFansCompletionService;
        private readonly SharePickerService _sharePickerService;

        private IDisposable _composeHandler;

        public bool IsPrivateView { get; set; }

        public Visibility LockVisibility { get; set; }

        public BindableCollection<CompletionUser> SelectedUsers
        {
            get { return _friendsFansCompletionService.SelectedItems; }
        }

        public bool IsSelectionEnabled { get; set; }

        [DependsOn("IsSelectionEnabled")]
        public double SelectionOpacity
        {
            get { return IsSelectionEnabled ? 1.0 : 0.5; }
        }

        public ComposePageViewModel(
            IPlurkService plurkService,
            INavigationService navigationService,
            IProgressService progressService,
            FriendsFansCompletionService friendsFansCompletionService,
            RecentEmoticonsService recentEmoticonsService,
            SharePickerService sharePickerService)
            : base (plurkService, progressService, recentEmoticonsService)
        {
            _navigationService = navigationService;
            _friendsFansCompletionService = friendsFansCompletionService;
            _sharePickerService = sharePickerService;

            LockVisibility = Visibility.Collapsed;
        }

        public override ISwitchControl GetSwitchView()
        {
            return GetView() as ISwitchControl;
        }

        protected override void OnActivate()
        {
            NotifyOfPropertyChange(() => SelectedUsers);
            if (SelectedUsers.Count > 0)
            {
                IsPrivateView = true;
                LockVisibility = Visibility.Visible;
                HasPostContentFocus = false;
            }

            base.OnActivate();

            if (_sharePickerService.ProcessAction)
            {
                UploadPicture(_sharePickerService.FileId);
                _sharePickerService.SetActionProcessed(true);
            }
        }

        protected override void OnPictureUploadFailed(PlurkError error)
        {
            if (LoginHelper.IsLoginError(error))
            {
                _sharePickerService.SetActionProcessed(false);
            }
        }

        protected override void OnEmoticonsLoaded()
        {
            LeaveFocus();
        }

        public override void Compose()
        {
            if (_composeHandler != null)
            {
                _composeHandler.Dispose();
            }
            if (String.IsNullOrWhiteSpace(PostContent))
            {
                MessageBox.Show(AppResources.tooManyCharacters);
            }
            else
            {
                ProgressService.Show(AppResources.msgSending);
                IsControlEnabled = false;

                var limitedTo = null as IEnumerable<int>;
                if (IsPrivateView)
                {
                    if (SelectedUsers.Count > 0)
                    {
                        limitedTo = SelectedUsers.Select(u => u.Id);
                    }
                    else
                    {
                        limitedTo = new int[] { 0 };
                    }
                }

                var lang = LanguageHelper.CultureInfoToPlurkLang(LocalizedStrings.Culture);

                var command =
                    TimelineCommand.PlurkAdd(PostContent, Qualifier.Qualifier, limitedTo, lang: lang)
                        .Client(PlurkService.Client)
                        .ToObservable()
                        .PlurkException(expectedTimeout: DefaultConfiguration.TimeoutCompose);

                _composeHandler = command.ObserveOnDispatcher().Subscribe(
                    plurk =>
                    {
                        var page = IoC.Get<PlurkMainPageViewModel>();
                        if (page != null)
                        {
                            page.NewPost = true;
                        }
                        PostContent = "";
                        SelectedUsers.Clear();
                        _navigationService.GoBack();
                    }, () =>
                    {
                        ProgressService.Hide();
                        IsControlEnabled = true;
                    });
            }
        }

        private void LeaveFocus()
        {
            var view = GetView() as ComposePage;
            if (view != null)
            {
                view.Focus();
            }
        }

        #region View Events

        public void OnPrivateChecked()
        {
            IsSelectionEnabled = true;
        }

        public void OnPrivateUnchecked()
        {
            IsSelectionEnabled = false;
        }

        public void ChooseFriends()
        {
            _navigationService.Navigate(new Uri("//Views/Compose/FriendsSelectionPage.xaml", UriKind.Relative));
        }

        #endregion

        #region App Bar

        public void PlurkAppBar()
        {
            Compose();
        }

        public void EmoticonAppBar()
        {
            LockVisibility = Visibility.Collapsed;
            EmoticonVisibility = Visibility.Visible;

            LoadEmoticons();

            LeaveFocus();
        }

        public void PhotosAppBar()
        {
            InsertPhoto();
        }

        public void PrivateAppBar()
        {
            EmoticonVisibility = Visibility.Collapsed;
            LockVisibility = Visibility.Visible;

            LeaveFocus();
        }

        #endregion
    }
}
