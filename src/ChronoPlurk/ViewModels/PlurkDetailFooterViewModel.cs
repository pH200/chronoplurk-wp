using System;
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
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using NotifyPropertyWeaver;
using Plurto.Commands;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class PlurkDetailFooterViewModel : Screen
    {
        private readonly IProgressService _progressService;

        private IPlurkService PlurkService { get; set; }

        private IDisposable _composeHandler;

        public string Content { get; set; }
        
        [DependsOn("Content")]
        public string TextCountLeft
        {
            get
            {
                var length = 0;
                if (Content != null)
                {
                    length = Content.Length;
                }

                var count = 140 - length;
                return count >= 0 ? count.ToString() : "Too many characters";
            }
        }

        public bool ResponseFocus { get; set; }

        public PlurkDetailFooterViewModel
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
            _progressService.Show("sending");

            // TODO: url encode
            _composeHandler =
                ResponsesCommand.ResponseAdd(GetPlurkId(), Content, Qualifier.FreestyleColon).Client(PlurkService.Client)
                    .ToObservable().Timeout(
                        TimeSpan.FromSeconds(20)).PlurkException(error => { }).ObserveOnDispatcher().Subscribe(
                            plurk => RefreshTimeline(), () => _progressService.Hide());
        }

        private int GetPlurkId()
        {
            var parent = Parent as PlurkDetailViewModel;
            if (parent == null)
            {
                return -1;
            }
            return parent.ListHeader.Id;
        }

        private void RefreshTimeline()
        {
            var parent = Parent as PlurkDetailViewModel;
            if (parent == null)
            {
                return;
            }
            // TODO: Add responses from bottom.
            parent.RefreshSync();
        }
    }
}