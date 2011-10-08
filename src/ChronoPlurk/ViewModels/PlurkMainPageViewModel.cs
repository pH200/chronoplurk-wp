using System;
using System.Linq;
using System.Reactive.Linq;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.Views;
using ChronoPlurk.Helpers;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
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
            Username = PlurkService.Username;
            _timeline.RefreshSync();
        }
        
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = view as PlurkMainPage;
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