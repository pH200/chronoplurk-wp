using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Caliburn.Micro;
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

        private IDisposable _requestHandler;

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

            IsLoginEnabled = true;
        }

        protected override void OnActivate()
        {
#if CLEAN_DEBUG
            Username = PlurkResources.Username;
            Password = PlurkResources.Password;
#else
            Username = _plurkService.Username;
#endif
            base.OnActivate();
        }

        public void UsernameKey(KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                var view = GetView() as LoginPage;
                if (view != null)
                {
                    view.Password.Focus();
                }
            }
        }

        public void PasswordKey(KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                Login();
            }
        }

        public void Login()
        {
            if (_requestHandler != null)
            {
                _requestHandler.Dispose();
            }
            IsLoginEnabled = false;
            _progressService.Show("Connecting");
            _requestHandler = _plurkService.LoginAsnc(Username, Password)
                .PlurkException(error => { }, _progressService)
                .ObserveOnDispatcher()
                .Subscribe(message =>
                {
                    _plurkService.SaveUserData();
                    _progressService.Hide();
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
                }, () => IsLoginEnabled = true);
        }
    }
}
