using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.Helpers;
using NotifyPropertyWeaver;
using Plurto.Commands;
using Plurto.Core;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class ComposePageViewModel : LoginAvailablePage
    {
        private IPlurkService PlurkService { get; set; }
        private readonly INavigationService _navigationService;
        private readonly IProgressService _progressService;
        private IDisposable _composeHandler;

        public string Username { get { return PlurkService.Username; } }

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

        public QualifierViewModel Qualifier { get; set; }

        public IList<QualifierViewModel> Qualifiers { get; private set; }

        public ComposePageViewModel
            (IPlurkService plurkService,
            INavigationService navigationService,
            IProgressService progressService,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            PlurkService = plurkService;
            _navigationService = navigationService;
            _progressService = progressService;

            Qualifiers = QualifierViewModel.AllQualifiers;
            Qualifier = QualifierViewModel.AllQualifiers.First(q => q.Qualifier == Plurto.Core.Qualifier.Says);
        }

        public void PlurkAppBar()
        {
            Compose();
        }

        public void Compose()
        {
            if (_composeHandler != null)
            {
                _composeHandler.Dispose();
            }
            if (String.IsNullOrWhiteSpace(Content))
            {
                MessageBox.Show("Write something before you plurk!");
            }
            else
            {
                _progressService.Show("Sending");

                var command =
                    TimelineCommand.PlurkAdd(Content, Qualifier.Qualifier).
                        Client(PlurkService.Client).ToObservable().
                        Timeout(TimeSpan.FromSeconds(20)).
                        PlurkException(error => { });

                _composeHandler = command.ObserveOnDispatcher().Subscribe(
                    plurk =>
                    {
                        var page = IoC.Get<PlurkMainPageViewModel>();
                        if (page != null)
                        {
                            page.NewPost = true;
                        }
                        Content = "";
                        _navigationService.GoBack();
                    }, () => _progressService.Hide());
            }
        }
    }
}
