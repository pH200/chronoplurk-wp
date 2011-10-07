using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ChronoPlurk.Services;
using ChronoPlurk.Views;
using ChronoPlurk.Helpers;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class LoginViewModel : Screen, IChildT<ILoginAvailablePage>
    {
        private readonly IPlurkService _plurkService;
        private readonly IProgressService _progressService;
        private readonly INavigationService _navigationService;

        private LoginView _view;

        private IDisposable _requestHandler;

        public string Username { get; set; }

        public string Password { get; set; }

        public string Message { get; set; }

        public Uri RedirectUri { get; set; }

        public LoginViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService)
        {
            _navigationService = navigationService;
            _progressService = progressService;
            _plurkService = plurkService;

#if CLEAN_DEBUG
            Username = PlurkResources.Username;
            Password = PlurkResources.Password;
#endif
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (LoginView) view;
        }

        public void UsernameKey(KeyEventArgs e)
        {
            if (e != null && e.Key == Key.Enter)
            {
                _view.Password.Focus();
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
            _progressService.Show("Connecting");
            _requestHandler = _plurkService.LoginAsnc(Username, Password).
                PlurkException(error => { }, _progressService).ObserveOnDispatcher().
                Subscribe(message =>
                {
                    _plurkService.SaveUserData();
                    _progressService.Hide();
                    if (RedirectUri != null)
                    {
                        _navigationService.Navigate(RedirectUri);
                        RedirectUri = null;
                    }
                    else
                    {
                        MessageBox.Show("Login Successful.");
                        var parent = this.GetParent();
                        parent.HideLoginPopup();
                    }
                });
        }
    }
}
