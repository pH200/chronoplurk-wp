using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using Microsoft.Phone.Tasks;
using NotifyPropertyWeaver;
using Plurto.Commands;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class PlurkDetailReplyViewModel : Screen, IChildT<PlurkDetailPageViewModel>
    {
        private readonly IProgressService _progressService;

        private IPlurkService PlurkService { get; set; }

        private IDisposable _composeHandler;
        private IDisposable _uploadHandler;
        private IDisposable _photoChooserDisposable;

        private string _textBoxDistinctKey;

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
                return count >= 0 ? count.ToString() : AppResources.tooManyCharacters;
            }
        }

        public bool ResponseFocus { get; set; }

        public bool OpeningPhotoChooser { get; set; }

        public PlurkDetailReplyViewModel
            (IPlurkService plurkService,
            IProgressService progressService)
        {
            PlurkService = plurkService;
            _progressService = progressService;
        }

        public void Reply()
        {
            if (_composeHandler != null)
            {
                _composeHandler.Dispose();
            }
            if (String.IsNullOrWhiteSpace(PostContent))
            {
                MessageBox.Show(AppResources.warnEmpty);
            }
            else
            {
                var plurkId = GetPlurkId();
                if (plurkId != -1)
                {
                    _progressService.Show(AppResources.msgSending);
                    LeaveFocus();

                    var command =
                        ResponsesCommand.ResponseAdd(GetPlurkId(), PostContent, Qualifier.FreestyleColon).
                            Client(PlurkService.Client).
                            ToObservable().
                            Timeout(DefaultConfiguration.TimeoutCompose).
                            PlurkException(error => _progressService.Hide());

                    _composeHandler = command.ObserveOnDispatcher().Subscribe(plurk =>
                    {
                        var parent = this.GetParent();
                        if (parent != null)
                        {
                            parent.BlurReplyFocus();
                        }
                        PostContent = "";
                        LoadNewComments();
                    });
                }
            }
        }

        private long GetPlurkId()
        {
            var parent = this.GetParent();
            return parent != null ? parent.GetPlurkId() : -1;
        }

        private void LeaveFocus()
        {
            var view = GetView() as Control;
            if (view != null)
            {
                view.Focus();
            }
        }

        private void LoadNewComments()
        {
            var parent = this.GetParent();
            if (parent != null)
            {
                parent.LoadNewComments();
            }
        }

        public void OnTextChanged()
        {
            if (PostContent == _textBoxDistinctKey)
            {
                return;
            }
            else
            {
                _textBoxDistinctKey = PostContent;
                var page = GetPageViewModel();
                if (page != null)
                {
                    page.UpdateReplyButton();
                }
            }
        }

        public void OnGotFocus()
        {
            var page = GetPageViewModel();
            if (page != null)
            {
                page.ShowPhotosAppBar();
            }
        }

        public void OnLostFocus()
        {
            var page = GetPageViewModel();
            if (page != null)
            {
                page.HidePhotosAppBar();
            }
        }

        private PlurkDetailPageViewModel GetPageViewModel()
        {
            return this.GetParent();
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
                OpeningPhotoChooser = true;
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
    }
}
