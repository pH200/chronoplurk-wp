using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using Microsoft.Phone.Tasks;
using NotifyPropertyWeaver;
using Plurto.Commands;
using Plurto.Core;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels.Compose
{
    [NotifyForAll]
    public class ComposeViewModelBase : Conductor<IScreen>.Collection.OneActive
    {
        protected IPlurkService PlurkService { get; private set; }
        protected IProgressService ProgressService { get; private set; }
        protected RecentEmoticonsService RecentEmoticonsService { get; private set; }

        private IDisposable _uploadHandler;
        private IDisposable _photoChooserDisposable;
        private IDisposable _emoticonsDisposable;

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

        public bool HasPostContentFocus { get; set; }

        public BindableCollection<EmoticonListViewModel> Emoticons { get; set; }

        private EmoticonListViewModel RecentEmoticons { get; set; }

        public EmoticonListViewModel ActiveEmoticon { get; set; }

        public Visibility EmoticonVisibility { get; set; }

        public ComposeViewModelBase(
            IPlurkService plurkService,
            IProgressService progressService,
            RecentEmoticonsService recentEmoticonsService)
        {
            PlurkService = plurkService;
            ProgressService = progressService;
            RecentEmoticonsService = recentEmoticonsService;

            EmoticonVisibility = Visibility.Collapsed;
            HasPostContentFocus = true;

            Qualifiers = QualifierViewModel.AllQualifiers;
            Qualifier = QualifierViewModel.AllQualifiers.First(q => q.Qualifier == Plurto.Core.Qualifier.Says);
        }

        public void LoadEmoticons()
        {
            if (Emoticons == null)
            {
                RecentEmoticons = EmoticonListViewModel.CreateBindable(RecentEmoticonsService.List);
                RecentEmoticons.DisplayName = DisplayName = AppResources.emoticonsRecent;
                Emoticons = new BindableCollection<EmoticonListViewModel>()
                {
                    RecentEmoticons
                };
            }
            if (Emoticons.Count < 2)
            {
                if (_emoticonsDisposable != null)
                {
                    _emoticonsDisposable.Dispose();
                }
                ProgressService.Show(AppResources.msgLoadingEmoticons);
                var emoticonsCmd = EmoticonsCommand.Get().Client(PlurkService.Client)
                    .ToObservable()
                    .Retry(DefaultConfiguration.RetryCount)
                    .Catch<Emoticons, Exception>(e =>
                    {
                        Execute.OnUIThread(() => MessageBox.Show("Cannot load emoticons"));
                        return Observable.Empty<Emoticons>();
                    });
                _emoticonsDisposable = emoticonsCmd.Subscribe(emoticons =>
                {
                    var excludeList = new[] { "(v_shy)", "(xmas1)", "(xmas2)", "(xmas3)" };
                    var karmaValue = PlurkService.AppUserInfo.User.Karma;
                    var karmaEmoticons =
                        emoticons.GetKarmaEmoticons(karmaValue).Where(pair => !excludeList.Contains(pair.Key));
                    if (PlurkService.AppUserInfo.User.Recruited >= 10)
                    {
                        karmaEmoticons = karmaEmoticons.Concat(emoticons.GetRecuitedEmoticons());
                    }

                    var custom = new EmoticonListViewModel(emoticons.GetCustomBracktedEmoticons())
                    {
                        DisplayName = AppResources.emoticonsCustom
                    };
                    var karma = new EmoticonListViewModel(karmaEmoticons)
                    {
                        DisplayName = AppResources.emoticonsKarma
                    };
                    Emoticons.Add(custom);
                    Emoticons.Add(karma);

                    if (RecentEmoticons.Items.Count == 0)
                    {
                        ActiveEmoticon = custom.Items.Count != 0 ? custom : karma;
                    }

                }, () => Execute.OnUIThread(() =>
                {
                    ProgressService.Hide();
                    OnEmoticonsLoaded();
                }));
            }
        }

        protected virtual void OnEmoticonsLoaded()
        {
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

        protected override void OnDeactivate(bool close)
        {
            if (RecentEmoticons != null && RecentEmoticons.Items != null)
            {
                RecentEmoticonsService.Replace(RecentEmoticons.Items);
                RecentEmoticonsService.Save(PlurkService.UserId);
            }

            base.OnDeactivate(close);
        }

        public void OnEmoticonTapped(object dataContext)
        {
            var emoticon = (KeyValuePair<string, string>)dataContext;
            if (emoticon.Key != null)
            {
                PostContent += emoticon.Key;
                if (RecentEmoticons != null)
                {
                    RecentEmoticonsService.Insert(RecentEmoticons.Items, emoticon);
                }
            }
        }

        public virtual void Compose()
        {
        }

        public void InsertPhoto()
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
                    .PlurkException(expectedTimeout: DefaultConfiguration.TimeoutUpload);

                ProgressService.Show(AppResources.msgUploadingPhoto);

                System.Action complete = () =>
                {
                    ProgressService.Hide();
                    if (photoStream != null)
                    {
                        photoStream.Dispose();
                    }
                };

                _uploadHandler =
                    uploadCommand.ObserveOnDispatcher().Subscribe(
                        picture =>
                        {
                            PostContent += picture.Full + Environment.NewLine;
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
    }
}
