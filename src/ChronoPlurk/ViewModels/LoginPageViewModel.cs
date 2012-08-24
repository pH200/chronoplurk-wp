using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using ChronoPlurk.Views;
using ChronoPlurk.Helpers;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Phone.Tasks;
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

        public string DeviceName { get; set; }

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

            UpdateDeviceName();
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

            var verifier = _plurkService.VerifierTemp;
            _plurkService.VerifierTemp = null;

            var getAccessToken = from oauthClient in _plurkService.GetAccessToken(verifier)
                                 from userData in _plurkService.CreateUserData(oauthClient)
                                 select userData;

            _createDisposable = getAccessToken
                .DoProgress(_progressService, AppResources.msgConnecting)
                .PlurkException()
                .ObserveOnDispatcher()
                .Subscribe(unit => OnLoggedIn(), () =>
                {
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
            _navigationService.Navigate(new Uri("/Views/LoginBrowserPage.xaml?DeviceName=" + DeviceName, UriKind.Relative));
        }

        private void UpdateDeviceName()
        {
            DeviceName = Microsoft.Phone.Info.DeviceStatus.DeviceName;
        }

        public void UndoDevice()
        {
            UpdateDeviceName();
        }

        public void HelpDevice()
        {
            NotificationTool.Show(
                AppResources.helpTitle,
                AppResources.helpDeviceId.Replace("\\n", Environment.NewLine),
                new NotificationAction(AppResources.helpOK, () => { }),
                new NotificationAction(AppResources.helpExample, () =>
                {
                    var uri = new Uri("http://images.plurk.com/4c31662a172aad703ef9d5535458b77f.jpg", UriKind.Absolute);
                    _navigationService.GotoImageBrowserPage(uri);
                }));
        }
    }
}
