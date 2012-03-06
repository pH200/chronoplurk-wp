using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using ChronoPlurk.Helpers;
using ChronoPlurk.Views.ProgressBar;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ChronoPlurk.Services
{
    public interface IProgressService
    {
        void Show(string message = null);
        void Hide();
    }

    public class ProgressService : IProgressService
    {
        private PhoneApplicationPage _page;

        private readonly ProgressIndicator _progressIndicator = new ProgressIndicator()
        {
            IsVisible = true
        };

        public void Show(string message = null)
        {
            var activePage = Application.Current.GetActivePage();

            if (activePage != null)
            {
                if (_page != null && activePage != _page)
                {
                    SystemTray.SetProgressIndicator(_page, null);
                }

                _page = activePage;
                _progressIndicator.Text = message;
                _progressIndicator.IsIndeterminate = true;
                SystemTray.SetProgressIndicator(_page, _progressIndicator);
            } 
        }

        public void Hide()
        {
            _progressIndicator.IsIndeterminate = false;
            _progressIndicator.Text = null;
        }
    }
}
