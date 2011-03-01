using System;
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
using MetroPlurk.Services;
using Plurto.Commands;
using Plurto.Entities;

namespace MetroPlurk.ViewModels
{
    public class PlurkDetailViewModel : TimelineBaseViewModel<ResponsesResult>, IRefreshSync
    {
        public PlurkDetailHeaderViewModel ListHeader { get; private set; }

        public bool RefreshOnActivate { get; set; }

        public PlurkDetailViewModel
            (INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            PlurkDetailHeaderViewModel plurkDetailHeaderViewModel)
            : base(navigationService, progressService, plurkService)
        {
            ListHeader = plurkDetailHeaderViewModel;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (!RefreshOnActivate) return;
            RefreshOnActivate = false;
            RefreshSync();
        }

        public void RefreshSync()
        {
            var getPlurks =
                ResponsesCommand.Get(ListHeader.Id, 0, PlurkService.Cookie).
                LoadAsync();

            Request(getPlurks);
        }
    }
}
