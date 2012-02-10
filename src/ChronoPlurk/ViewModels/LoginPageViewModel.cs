﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Views;
using ChronoPlurk.Helpers;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class LoginPageViewModel : Screen
    {
        private readonly IPlurkService _plurkService;
        private readonly IProgressService _progressService;
        private readonly INavigationService _navigationService;
        private IDisposable _createDisposable;

        public string Username { get; set; }

        public string Password { get; set; }

        public string Message { get; set; }

        // See also NavigationExtensions.cs
        public bool RedirectMainPage { get; set; }

        public bool IsLoginEnabled { get; set; }

        public LoginPageViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService)
        {
            _navigationService = navigationService;
            _progressService = progressService;
            _plurkService = plurkService;
        }

        protected override void OnActivate()
        {
            if (_plurkService.VerifierTemp != null)
            {
                ProcessLogin();
            }
            else
            {
                IsLoginEnabled = true;
            }
            base.OnActivate();
        }

        private void ProcessLogin()
        {
            if (_createDisposable != null)
            {
                _createDisposable.Dispose();
            }
            IsLoginEnabled = false;
            _progressService.Show(AppResources.msgConnecting);

            var verifier = _plurkService.VerifierTemp;
            _plurkService.VerifierTemp = null;

            var getAccessToken = _plurkService
                .GetAccessToken(verifier)
                .Select(client => _plurkService.CreateUserData(client))
                .Merge();

            _createDisposable = getAccessToken
                .PlurkException(progressService: _progressService)
                .ObserveOnDispatcher()
                .Subscribe(unit => OnLoggedIn(), () =>
                {
                    _progressService.Hide();
                    IsLoginEnabled = true;
                });
        }
        
        private void OnLoggedIn()
        {
            _plurkService.SaveUserData();
            if (RedirectMainPage)
            {
                var mainPageUri = new Uri("/Views/PlurkMainPage.xaml", UriKind.Relative);
                if (_navigationService.CanGoBack)
                {
                    // Remove entry for MainPage.xaml
                    _navigationService.RemoveBackEntry();
                }
                _navigationService.SetRemoveBackEntryFlag();
                _navigationService.Navigate(mainPageUri);
            }
            else
            {
                if (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                }
                else
                {
                    throw new InvalidOperationException("Login page must be navigated from other pages.");
                }
            }
        }

        public void Login()
        {
            IsLoginEnabled = false;
            _navigationService.Navigate(new Uri("/Views/LoginBrowserPage.xaml", UriKind.Relative));
        }
    }
}
