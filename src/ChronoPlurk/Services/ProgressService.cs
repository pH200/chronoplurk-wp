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
        void Hide(int id);
        void Hide();
    }

    public class ProgressService : IProgressService
    {
        private PhoneApplicationPage _page;

        private int _idCounter = 1;

        private readonly Dictionary<int, string> _messages = new Dictionary<int, string>();

        private readonly ProgressIndicator _progressIndicator = new ProgressIndicator()
        {
            IsVisible = true
        };

        private int AddStack(string message)
        {
            var id = _idCounter;
            _messages.Add(id, message);
            _idCounter++;
            return id;
        }

        public int Show(string message = null)
        {
            ShowInternal(message);

            return AddStack(message);
        }

        private void ShowInternal(string message)
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
                    _progressIndicator.IsIndeterminate = true;
                    SystemTray.SetProgressIndicator(_page, _progressIndicator);
                }
            });
        }

        public void Hide(int id)
        {
            _messages.Remove(id);
            if (_messages.Count > 0)
            {
                var message = _messages.OrderByDescending(m => m.Key).First();
                ShowInternal(message.Value);
            }
            else
            {
                Hide();
            }
        }

        public void Hide()
        {
            _messages.Clear();
            ThreadEx.OnUIThread(() =>
            {
                _progressIndicator.IsIndeterminate = false;
                _progressIndicator.Text = null;
            });
        }
    }
}
