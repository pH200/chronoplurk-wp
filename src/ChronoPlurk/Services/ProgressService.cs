using System;
using System.Collections.Generic;
using System.Linq;
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
        int Show(string message = null);
        void ShowQuickMessage(string message);
        void Hide(int id);
        void Hide();
    }

    public class ProgressService : IProgressService
    {
        private PhoneApplicationPage _page;

        private int _idCounter = 1;

        private readonly Dictionary<int, string> _messages = new Dictionary<int, string>();

        private readonly object _gate = new object();

        private readonly ProgressIndicator _progressIndicator = new ProgressIndicator()
        {
            IsVisible = true
        };

        private int AddStack(string message)
        {
            lock (_gate)
            {
                var id = _idCounter;
                _messages.Add(id, message);
                _idCounter++;
                return id;
            }
        }

        public int Show(string message = null)
        {
            ShowInternal(message, true);
            return AddStack(message);
        }

        public void ShowQuickMessage(string message)
        {
            ShowInternal(message, false);
            var prgId = AddStack(message);
            ThreadEx.TimerAction(TimeSpan.FromSeconds(1.5), () => Hide(prgId));
        }

        private void ShowInternal(string message, bool isIndeterminate)
        {
            ThreadEx.OnUIThread(() =>
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
                    _progressIndicator.IsIndeterminate = isIndeterminate;
                    SystemTray.SetProgressIndicator(_page, _progressIndicator);
                }
            });
        }

        public void Hide(int id)
        {
            lock (_gate)
            {
                _messages.Remove(id);
                if (_messages.Count > 0)
                {
                    var message = _messages.OrderByDescending(m => m.Key).First();
                    ShowInternal(message.Value, true);
                    return;
                }
            }
            Hide();
        }

        public void Hide()
        {
            lock (_gate)
            {
                _messages.Clear();
            }
            ThreadEx.OnUIThread(() =>
            {
                _progressIndicator.IsIndeterminate = false;
                _progressIndicator.Text = null;
            });
        }
    }
}
