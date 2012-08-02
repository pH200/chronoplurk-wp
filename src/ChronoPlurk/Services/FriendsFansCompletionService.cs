using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Caliburn.Micro;
using ChronoPlurk.Helpers;
using Plurto.Commands;
using Plurto.Entities;

namespace ChronoPlurk.Services
{
    public class FriendsFansCompletionService
    {
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
            var observable = FriendsFansCommand.GetCompletion()
                .Client(PlurkService.Client)
                .ToObservable()
                .PlurkException();
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
        /// Save completion to isolated storage. Executes asynchronously.
        /// </summary>
        public void SaveCompletion()
        {
            if (Completion != null)
            {
                var filename = GetBinaryFileName();
                ThreadEx.OnThreadPool(() => IsoSettings.SerializeStore(Completion, filename));
            }
        }

        private string GetBinaryFileName()
        {
            // TODO: Ensure PlurkService.IsLoaded
            return "completion-" + PlurkService.UserId + ".bin";
        }
    }
}
