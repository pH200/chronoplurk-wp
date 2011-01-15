using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using MetroPlurk.Views;

namespace MetroPlurk.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {
                if (_username == value) return;
                _username = value;
                NotifyOfPropertyChange(() => Username);
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value) return;
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message == value) return;
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public Uri RedirectUri { get; set; }

        private LoginView _view;

        private readonly IPlurkService _plurkService;
        private readonly IProgressService _progressService;
        private readonly INavigationService _navigationService;

        private IDisposable _requestHandler;
        

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
                PlurkException(error=>{}, _progressService).ObserveOnDispatcher().
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
                        if (Parent is ILoginAvailablePage)
                        {
                            var parent = (ILoginAvailablePage)Parent;
                            parent.HideLoginPopup();
                        }
                    }
                });
        }
    }
}
