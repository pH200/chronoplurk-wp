using System;
using System.ComponentModel;
using Caliburn.Micro;
using MetroPlurk.Helpers;
using Microsoft.Phone.Controls;

namespace MetroPlurk.ViewModels
{
    public interface ILoginAvailablePage
    {
        LoginViewModel LoginViewModel { get; }
        bool IsLoginPopupOpen { get; }
        void ShowLoginPopup(bool locked, Uri redirect = null);
        void HideLoginPopup();
    }

    public class LoginAvailablePage : Conductor<IScreen>.Collection.OneActive, ILoginAvailablePage
    {
        public LoginViewModel LoginViewModel { get; private set; }
        private PhoneApplicationPage _view;
        private bool _locked;

        private double _popupWidth;

        public double PopupWidth
        {
            get { return _popupWidth; }
            set
            {
                if (_popupWidth == value) return;
                _popupWidth = value;
                NotifyOfPropertyChange(() => PopupWidth);
            }
        }

        private double _popupHeight;

        public double PopupHeight
        {
            get { return _popupHeight; }
            set
            {
                if (_popupHeight == value) return;
                _popupHeight = value;
                NotifyOfPropertyChange(() => PopupHeight);
            }
        }

        private bool _isLoginPopupOpen;

        public bool IsLoginPopupOpen
        {
            get { return _isLoginPopupOpen; }
            private set
            {
                if (_isLoginPopupOpen == value) return;
                _isLoginPopupOpen = value;
                NotifyOfPropertyChange(() => IsLoginPopupOpen);
            }
        }
        
        public LoginAvailablePage(LoginViewModel loginViewModel)
        {
            LoginViewModel = loginViewModel;
        }

        private void SetParent()
        {
            var node = LoginViewModel as IChild<IConductor>;
            if(node != null && node.Parent != this)
                node.Parent = this;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            HideLoginPopup();
            SetParent();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            _view = (PhoneApplicationPage) view;
            SetPopupSize(_view.Orientation);
            _view.OrientationChanged += ViewOrientationChanged;
            _view.BackKeyPress += BackKeyPress;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            HideLoginPopup();
        }

        private void SetPopupSize(PageOrientation orientation)
        {
            switch (orientation)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                case PageOrientation.LandscapeRight:
                    PopupWidth = PlurkResources.PhoneHeight;
                    PopupHeight = PlurkResources.PhoneWidth;
                    break;
                default:
                    PopupWidth = PlurkResources.PhoneWidth;
                    PopupHeight = PlurkResources.PhoneHeight;
                    break;
            }
        }

        void ViewOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            SetPopupSize(e.Orientation);
        }

        private void BackKeyPress(object sender, CancelEventArgs e)
        {
            if (!IsLoginPopupOpen || _locked) return;

            e.Cancel = true;
            HideLoginPopup();
        }

        public void ShowLoginPopup(bool locked, Uri redirect = null)
        {
            _locked = locked;
            if (redirect != null)
            {
                LoginViewModel.RedirectUri = redirect;
            }
            if (_view != null && _view.ApplicationBar != null)
            {
                _view.ApplicationBar.IsMenuEnabled = false;
                _view.ApplicationBar.IsVisible = false;
            }

            IsLoginPopupOpen = true;
        }

        public void HideLoginPopup()
        {
            if (_view != null && _view.ApplicationBar != null)
            {
                _view.ApplicationBar.IsMenuEnabled = true;
                _view.ApplicationBar.IsVisible = true;
            }

            IsLoginPopupOpen = false;
        }
    }
}