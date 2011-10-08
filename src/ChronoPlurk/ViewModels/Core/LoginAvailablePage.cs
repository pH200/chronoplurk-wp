using System;
using System.ComponentModel;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using Microsoft.Phone.Controls;
using NotifyPropertyWeaver;

namespace ChronoPlurk.ViewModels
{
    public interface ILoginAvailablePage
    {
        LoginViewModel LoginViewModel { get; }
        bool IsLoginPopupOpen { get; }
        void ShowLoginPopup(bool locked, Uri redirect = null);
        void HideLoginPopup();
    }

    [NotifyForAll]
    public class LoginAvailablePage : Conductor<IScreen>.Collection.OneActive, ILoginAvailablePage
    {
        private PhoneApplicationPage _view;

        private bool _locked;

        public LoginViewModel LoginViewModel { get; private set; }
        
        public double PopupWidth { get; set; }

        public double PopupHeight { get; set; }

        public bool IsLoginPopupOpen { get; private set; }

        public bool IsMenuEnabled { get; set; }
        
        public LoginAvailablePage(LoginViewModel loginViewModel)
        {
            LoginViewModel = loginViewModel;
            IsMenuEnabled = true;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SetParent();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            HideLoginPopup();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            
            if (IsMenuEnabled)
            {
                ShowApplicationBar();
            }
            if (_view == null)
            {
                _view = (PhoneApplicationPage)view;
                SetPopupSize(_view.Orientation);
                _view.OrientationChanged += ViewOrientationChanged;
                _view.BackKeyPress += BackKeyPress;
            }
        }
        
        private void SetParent()
        {
            if (LoginViewModel != null && LoginViewModel.Parent != this)
            {
                LoginViewModel.Parent = this;
            }
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

        private void ViewOrientationChanged(object sender, OrientationChangedEventArgs e)
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

            (LoginViewModel as IActivate).Activate();
            IsLoginPopupOpen = true;
        }

        public void HideLoginPopup()
        {
            (LoginViewModel as IDeactivate).Deactivate(false);
            ShowApplicationBar();

            IsLoginPopupOpen = false;
        }

        private void ShowApplicationBar()
        {
            if (_view != null && _view.ApplicationBar != null)
            {
                _view.ApplicationBar.IsMenuEnabled = true;
                _view.ApplicationBar.IsVisible = true;
            }
        }
    }
}