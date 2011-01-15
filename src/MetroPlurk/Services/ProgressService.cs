using System;
using System.Windows.Controls.Primitives;
using MetroPlurk.Helpers;
using MetroPlurk.Views.ProgressBar;

namespace MetroPlurk.Services
{
    public interface IProgressService
    {
        void Show();
        void Show(string message);
        void Hide();
    }

    public class ProgressService : IProgressService
    {
        private readonly Popup _popup;
        private readonly ProgressView _progressView;
        private const string DefaultMessage = "Loading";

        public ProgressService()
        {
            _progressView = new ProgressView();
            _popup = new Popup
            {
                VerticalOffset = 0,
                Child = _progressView
            };
        }

        public void Show()
        {
            Show(DefaultMessage);
        }

        public void Show(string message)
        {
            _popup.IsOpen = true;
            _progressView.Width = PlurkResources.PhoneWidth;
            _progressView.Message.Text = message;
        }

        public void Hide()
        {
            _popup.IsOpen = false;
        }
    }
}
