using System;
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
using ChronoPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Commands;
using Plurto.Entities;

namespace ChronoPlurk.ViewModels
{
    [NotifyForAll]
    public class PlurkDetailViewModel : TimelineBaseViewModel<ResponsesResult>, IRefreshSync
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

        public PlurkDetailViewModel
            (INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            PlurkDetailHeaderViewModel plurkDetailHeaderViewModel,
            PlurkDetailFooterViewModel plurkDetailFooterViewModel)
            : base(navigationService, progressService, plurkService)
        {
            plurkDetailHeaderViewModel.Parent = this;
            DetailHeader = plurkDetailHeaderViewModel;
            plurkDetailFooterViewModel.Parent = this;
            DetailFooter = plurkDetailFooterViewModel;
            IgnoreSelection = true;
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

        public void RefreshSync()
        {
            var getPlurks = ResponsesCommand.Get(DetailHeader.Id, 0).Client(PlurkService.Client).ToObservable();

            Request(getPlurks);
        }
    }
}
