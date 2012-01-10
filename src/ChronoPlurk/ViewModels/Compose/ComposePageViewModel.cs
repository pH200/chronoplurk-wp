using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using Microsoft.Phone.Tasks;
using NotifyPropertyWeaver;
using Plurto.Commands;
using Plurto.Core;

namespace ChronoPlurk.ViewModels.Compose
{
    [NotifyForAll]
    public class ComposePageViewModel : LoginAvailablePage, IHandle<TaskCompleted<PhotoResult>>
    {
        private IPlurkService PlurkService { get; set; }
        private readonly INavigationService _navigationService;
        private readonly IProgressService _progressService;
        private readonly IEventAggregator _events;

        private IDisposable _composeHandler;
        private IDisposable _uploadHandler;

        public string Username { get { return PlurkService.Username; } }

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
                return count >= 0 ? count.ToString() : "Too many characters";
            }
        }

        public QualifierViewModel Qualifier { get; set; }

        public IList<QualifierViewModel> Qualifiers { get; private set; }

        public ComposePageViewModel(
            IPlurkService plurkService,
            INavigationService navigationService,
            IProgressService progressService,
            IEventAggregator events,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            PlurkService = plurkService;
            _navigationService = navigationService;
            _progressService = progressService;

            _events = events;

            Qualifiers = QualifierViewModel.AllQualifiers;
            Qualifier = QualifierViewModel.AllQualifiers.First(q => q.Qualifier == Plurto.Core.Qualifier.Says);
        }

        protected override void OnActivate()
        {
            _events.Subscribe(this);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            _events.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        public void Compose()
        {
            if (_composeHandler != null)
            {
                _composeHandler.Dispose();
            }
            if (String.IsNullOrWhiteSpace(PostContent))
            {
                MessageBox.Show("Write something before you plurk!");
            }
            else
            {
                _progressService.Show("Sending");

                var command =
                    TimelineCommand.PlurkAdd(PostContent, Qualifier.Qualifier).
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
                        _navigationService.GoBack();
                    }, () => _progressService.Hide());
            }
        }

        private void InsertPhoto()
        {
            try
            {
                _events.RequestTask<PhotoChooserTask>(task =>
                {
                    task.ShowCamera = true;
                });
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("An error occured.");
#if DEBUG
                throw;
#endif
            }
        }

        public void Handle(TaskCompleted<PhotoResult> message)
        {
            if (message.Result.TaskResult == TaskResult.OK)
            {
                UploadPicture(message.Result.ChosenPhoto, message.Result.OriginalFileName);
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

                _progressService.Show("Uploading Photo");

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
                            PostContent += Environment.NewLine + picture.Full;
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

        #region App Bar

        public void PlurkAppBar()
        {
            Compose();
        }

        public void PhotosAppBar()
        {
            InsertPhoto();
        }

        #endregion
    }
}
