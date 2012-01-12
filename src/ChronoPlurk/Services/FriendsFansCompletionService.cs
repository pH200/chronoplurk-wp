using System;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Helpers;
using ChronoPlurk.ViewModels.Compose;
using Plurto.Commands;
using Plurto.Entities;

namespace ChronoPlurk.Services
{
    public class FriendsFansCompletionService
    {
        private IDisposable _saveDisposable;

        protected IPlurkService PlurkService { get; set; }

        public FriendsFansCompletion Completion { get; private set; }

        public bool IsLoaded { get; private set; }

        public bool IsDownloaded { get { return Completion != null; } }

        public BindableCollection<CompletionUser> SelectedItems { get; set; }

        public int LoadedUserId { get; set; }

        public FriendsFansCompletionService(IPlurkService plurkService)
        {
            PlurkService = plurkService;
            SelectedItems = new BindableCollection<CompletionUser>();
        }

        public void Load()
        {
            var filename = GetBinaryFileName();
            Completion = IsoSettings.DeserializeLoad(filename) as FriendsFansCompletion;
            if (Completion != null)
            {
                LoadedUserId = PlurkService.UserId;
            }
            IsLoaded = true;
        }

        public IConnectableObservable<FriendsFansCompletion> DownloadAsync()
        {
            var observable = FriendsFansCommand.GetCompletion().Client(PlurkService.Client).ToObservable();
            var connectable = observable.Publish();
            connectable.Subscribe(completion =>
            {
                LoadedUserId = PlurkService.UserId;
                Completion = completion;
                SaveCompletion();
            });
            return connectable;
        }

        /// <summary>
        /// Save completion to isolated storage. Run on current thread.
        /// </summary>
        public void SaveCompletion()
        {
            if (Completion != null)
            {
                if (_saveDisposable != null)
                {
                    _saveDisposable.Dispose();
                }
                var filename = GetBinaryFileName();
                var observable = Observable.Start(() => IsoSettings.SerializeStore(Completion, filename));
                _saveDisposable = observable.Subscribe();
            }
        }

        private string GetBinaryFileName()
        {
            // TODO: Ensure PlurkService.IsLoaded
            return "completion-" + PlurkService.UserId + ".bin";
        }
    }
}
