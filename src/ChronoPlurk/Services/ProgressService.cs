using System;
using System.Windows.Controls.Primitives;
using ChronoPlurk.Helpers;
using ChronoPlurk.Views.ProgressBar;
using Microsoft.Phone.Shell;

namespace ChronoPlurk.Services
{
    public interface IProgressService
    {
        void Show();
        void Show(string message);
        void Hide();
    }

    public class ProgressService : IProgressService
    {
        private readonly ProgressIndicator _progressIndicator;

        public ProgressService()
        {
            _progressIndicator = new ProgressIndicator() {IsVisible = true};
            SystemTray.ProgressIndicator = _progressIndicator;
        }

        public void Show()
        {
            Show(null);
        }

        public void Show(string message)
        {
            _progressIndicator.Text = message;
            _progressIndicator.IsIndeterminate = true;
        }

        public void Hide()
        {
            _progressIndicator.IsIndeterminate = false;
            _progressIndicator.Text = null;
        }
    }
}
