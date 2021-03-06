﻿using System;
using System.Collections;
using System.Linq;
using System.Net;
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
using ChronoPlurk.Resources.i18n;
using ChronoPlurk.Services;
using PropertyChanged;
using Plurto.Commands;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels
{
    [ImplementPropertyChanged]
    public class PlurkDetailViewModel : TimelineBaseViewModel<ResponsesResult>, IRefreshSync, IChildT<PlurkDetailPageViewModel>
    {
        [DependsOn("DetailHeader")]
        public override sealed object ListHeader { get { return DetailHeader; } }

        [DependsOn("DetailFooter")]
        public override sealed object ListFooter
        {
            get
            {
                {
                    return PlurkService.IsLoaded ? DetailFooter : _emptyViewModel;
                }
            }
        }
        private readonly object _emptyViewModel = new EmptyViewModel();

        public PlurkDetailHeaderViewModel DetailHeader { get; private set; }
        
        public PlurkDetailFooterViewModel DetailFooter { get; private set; }

        public bool RefreshOnActivate { get; set; }

        public PlurkDetailViewModel(
            INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            IPlurkContentStorageService plurkContentStorageService,
            PlurkDetailHeaderViewModel plurkDetailHeaderViewModel,
            PlurkDetailFooterViewModel plurkDetailFooterViewModel)
            : base(navigationService, progressService, plurkService, plurkContentStorageService)
        {
            plurkDetailHeaderViewModel.Parent = this;
            DetailHeader = plurkDetailHeaderViewModel;
            plurkDetailFooterViewModel.Parent = this;
            DetailFooter = plurkDetailFooterViewModel;

            IsCompareIdInsteadOfPlurkId = true; // Replies' plurk_id are thread id.
            DisableTimelinePlurkHolder = true;
            EnableHyperlink = true;

            ProgressMessage = AppResources.msgLoadingResponses;
        }

        protected override void OnActivate()
        {
            NotifyOfPropertyChange("ListFooter");
            if (RefreshOnActivate)
            {
                RefreshOnActivate = false;
                RefreshSync();
            }

            base.OnActivate();
        }

        public override void OnItemTap(object dataContext)
        {
            var item = dataContext as PlurkItemViewModel;
            if (item != null)
            {
                var reply = string.Format("@{0}: ", item.NickName);
                this.GetParent().AddTextToReply(reply);
            }
        }

        public void RefreshSync()
        {
            var getPlurks = ResponsesCommand.Get(DetailHeader.PlurkId, 0).Client(PlurkService.Client).ToObservable();

            Request(getPlurks);
            ScrollToTop();
        }

        public void LoadNewComments()
        {
            var newResponseOffset = Items.Count;
            var getPlurks = ResponsesCommand.Get(DetailHeader.PlurkId, newResponseOffset).Client(PlurkService.Client).ToObservable();

            var specialFallback = new SpecialFallback<ResponsesResult>(
                predicate: result =>
                {
                    return (result == null) ||
                           (result.Users == null) ||
                           (!result.Users.ContainsKey(PlurkService.UserId));
                },
                fallback: () => Execute.OnUIThread(this.RefreshSync));

            AppendRequest(getPlurks, specialFallback);
        }
    }
}
