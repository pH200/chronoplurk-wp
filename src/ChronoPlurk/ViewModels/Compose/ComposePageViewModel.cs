using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using ChronoPlurk.Views.Compose;
using Microsoft.Phone.Tasks;
using NotifyPropertyWeaver;
using Plurto.Commands;
using Plurto.Core;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Compose
{
    [NotifyForAll]
    public class ComposePageViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private IPlurkService PlurkService { get; set; }
        private readonly INavigationService _navigationService;
        private readonly IProgressService _progressService;
        private readonly FriendsFansCompletionService _friendsFansCompletionService;

        private IDisposable _composeHandler;
        private IDisposable _uploadHandler;
        private IDisposable _photoChooserDisposable;

        public string Username { get; set; }

        public string UserAvatar { get; set; }

        public string PostContent { get; set; }

        [DependsOn("PostContent")]
        public string TextCountLeft
        {
            get
            {
                var length = 0;
                if (PostContent != null)
                {
                    length = PostContent.Length;
                }

                var count = 140 - length;
                return count >= 0 ? count.ToString() : AppResources.warnEmpty;
            }
        }

        public QualifierViewModel Qualifier { get; set; }

        public IList<QualifierViewModel> Qualifiers { get; private set; }

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

        public bool HasPostContentFocus { get; set; }

        public IEnumerable<IDictionary<string, string>> Emoticons { get; set; }

        public Visibility EmoticonVisibility { get; set; }

        public ComposePageViewModel(
            IPlurkService plurkService,
            INavigationService navigationService,
            IProgressService progressService,
            FriendsFansCompletionService friendsFansCompletionService)
        {
            PlurkService = plurkService;
            _navigationService = navigationService;
            _progressService = progressService;
            _friendsFansCompletionService = friendsFansCompletionService;

            LockVisibility = Visibility.Collapsed;
            EmoticonVisibility = Visibility.Collapsed;
            HasPostContentFocus = true;

            Qualifiers = QualifierViewModel.AllQualifiers;
            Qualifier = QualifierViewModel.AllQualifiers.First(q => q.Qualifier == Plurto.Core.Qualifier.Says);
        }
        
        private void LoadEmoticons()
        {
            if (Emoticons == null)
            {
                _progressService.Show(AppResources.msgLoadingEmoticons);
                var emoticonsCmd = EmoticonsCommand.Get().Client(PlurkService.Client)
                    .ToObservable()
                    .Catch<Emoticons, Exception>(e =>
                    {
                        Execute.OnUIThread(() => MessageBox.Show("Cannot load emoticons"));
                        return Observable.Empty<Emoticons>();
                    });
                emoticonsCmd.Subscribe(emoticons =>
                {
                    Emoticons = new[]
                    {
                        emoticons.GetKarmaEmoticons(),
                        emoticons.GetRecuitedEmoticons()
                    };
                }, () => Execute.OnUIThread(() => _progressService.Hide()));
            }
        }

        protected override void OnInitialize()
        {
            Username = PlurkService.Username;
            if (PlurkService.AppUserInfo != null)
            {
                UserAvatar = PlurkService.AppUserInfo.UserAvatar;
            }

            base.OnInitialize();
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
        }

        public void OnEmoticonTapped(object dataContext)
        {
            var emoticon = (KeyValuePair<string, string>)dataContext;
            if (emoticon.Key != null)
            {
                PostContent += emoticon.Key;
            }
        }

        public void Compose()
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
                _progressService.Show(AppResources.msgSending);

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
                    TimelineCommand.PlurkAdd(PostContent, Qualifier.Qualifier, limitedTo, lang:lang).
                        Client(PlurkService.Client).ToObservable().
                        Timeout(DefaultConfiguration.TimeoutCompose).
                        PlurkException(error => { });

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
                    }, () => _progressService.Hide());
            }
        }

        private void InsertPhoto()
        {
            try
            {
                if (_photoChooserDisposable != null)
                {
                    _photoChooserDisposable.Dispose();
                }
                var photoChooser = new PhotoChooserTask() { ShowCamera = true };
                var pattern = Observable.FromEventPattern<PhotoResult>(handler => photoChooser.Completed += handler,
                                                                       handler => photoChooser.Completed -= handler);
                _photoChooserDisposable = pattern.Take(1).TimeInterval().Subscribe(result =>
                {
                    var photoResult = result.Value.EventArgs;
                    switch (photoResult.TaskResult)
                    {
                        case TaskResult.OK:
                            UploadPicture(photoResult.ChosenPhoto, photoResult.OriginalFileName);
                            break;
                        case TaskResult.Cancel:
                            if (result.Interval < TimeSpan.FromSeconds(1.5))
                            {
                                Execute.OnUIThread(() => MessageBox.Show(AppResources.msgDisconnectPC));
                            }
                            break;
                    }
                });
                photoChooser.Show();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(AppResources.errorOccured);
#if DEBUG
                throw;
#endif
            }
        }

        private void UploadPicture(Stream pictureStream, string filename)
        {
            if (_uploadHandler != null)
            {
                _uploadHandler.Dispose();
            }
            var photoStream = pictureStream;
            var shortFileName = filename.Split(new[] { '\\', '/' }).LastOrDefault();
            if (shortFileName != null)
            {
                var upload = new UploadFile(photoStream, shortFileName);
                SetUploadFileContentType(upload, shortFileName);
                var uploadCommand = TimelineCommand.UploadPicture(upload)
                    .Client(PlurkService.Client)
                    .ToObservable()
                    .Timeout(DefaultConfiguration.TimeoutUpload)
                    .PlurkException();

                _progressService.Show(AppResources.msgUploadingPhoto);

                System.Action complete = () =>
                {
                    _progressService.Hide();
                    if (photoStream != null)
                    {
                        photoStream.Dispose();
                    }
                };

                _uploadHandler =
                    uploadCommand.ObserveOnDispatcher().Subscribe(
                        picture =>
                        {
                            PostContent += picture.Full;
                        }, complete);
            }
        }

        private static void SetUploadFileContentType(UploadFile upload, string fileName)
        {
            if (fileName.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
                fileName.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase))
            {
                upload.ContentType = "image/jpeg";
            }
            else if (fileName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
            {
                upload.ContentType = "image/png";
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

            var view = GetView() as ComposePage;
            if (view != null)
            {
                view.Focus();
            }
        }

        public void PhotosAppBar()
        {
            InsertPhoto();
        }

        public void PrivateAppBar()
        {
            EmoticonVisibility = Visibility.Collapsed;
            LockVisibility = Visibility.Visible;

            var view = GetView() as ComposePage;
            if (view != null)
            {
                view.Focus();
            }
        }

        #endregion
    }
}
