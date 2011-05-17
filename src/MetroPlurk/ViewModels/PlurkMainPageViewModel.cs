using System;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using MetroPlurk.Views;
using NotifyPropertyWeaver;

namespace MetroPlurk.ViewModels
{
    [NotifyForAll]
    public sealed class PlurkMainPageViewModel : PlurkAppBarPage
    {
        private readonly TimelineViewModel _timeline;

        private PlurkMainPage _view;
        
        public string Username { get; set; }
        
        public PlurkMainPageViewModel(
            INavigationService navigationService,
            IPlurkService plurkService,
            LoginViewModel loginViewModel,
            TimelineViewModel timeline)
            : base(navigationService, plurkService, loginViewModel)
        {
            _timeline = timeline;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.Add(_timeline);
            ActivateItem(_timeline);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (!PlurkService.IsLoaded)
            {
                InitializePlurkLogin();
            }
            else
            {
                InitializeAfterLogin();
            }
        }
        
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = view as PlurkMainPage;
        }

        private void InitializePlurkLogin()
        {
            PlurkService.LoadUserData();
            PlurkService.LoginAsnc().PlurkException(
                error => NavigationService.Navigate(new Uri("/Views/LoginViewModel.xaml", UriKind.Relative))).
                ObserveOnDispatcher().Subscribe(message => InitializeAfterLogin());
        }

        private void InitializeAfterLogin()
        {
            Username = PlurkService.Username;
            _timeline.RefreshSync();
        }

        public void RefreshAppBar()
        {
            foreach (var screen in Items.OfType<IRefreshSync>())
            {
                if (screen == ActiveItem)
                {
                    screen.RefreshSync();
                }
                else
                {
                    screen.RefreshOnActivate = true;
                }
            }
        }

        public void SignOutAppBar()
        {
            PlurkService.ClearUserData();
            ShowLoginPopup(true);
        }
    }
}