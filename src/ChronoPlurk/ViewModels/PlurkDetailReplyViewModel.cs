using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using ChronoPlurk.ViewModels.Compose;
using NotifyPropertyWeaver;
using Plurto.Commands;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class PlurkDetailReplyViewModel : ComposeViewModelBase, IChildT<PlurkDetailPageViewModel>
    {
        private IDisposable _composeHandler;

        private string _textBoxDistinctKey;

        public bool ResponseFocus { get; set; }

        public PlurkDetailReplyViewModel(
            IPlurkService plurkService,
            IProgressService progressService,
            RecentEmoticonsService recentEmoticonsService)
            : base(plurkService, progressService, recentEmoticonsService)
        {
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
                MessageBox.Show(AppResources.warnEmpty);
            }
            else
            {
                var plurkId = GetPlurkId();
                if (plurkId != -1)
                {
                    ProgressService.Show(AppResources.msgSending);
                    LeaveFocus();

                    var command =
                        ResponsesCommand.ResponseAdd(GetPlurkId(), PostContent, Plurto.Core.Qualifier.FreestyleColon)
                            .Client(PlurkService.Client)
                            .ToObservable()
                            .Timeout(DefaultConfiguration.TimeoutCompose)
                            .PlurkException(error => ProgressService.Hide());

                    _composeHandler = command.ObserveOnDispatcher().Subscribe(plurk =>
                    {
                        var parent = this.GetParent();
                        if (parent != null)
                        {
                            LeaveFocus();
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
            var parent = this.GetParent();
            if (parent != null)
            {
                var view = parent.GetView() as Control;
                if (view != null)
                {
                    view.Focus();
                }
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
    }
}
