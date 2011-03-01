using System;
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
using Plurto.Entities;

namespace MetroPlurk.ViewModels
{
    public class PlurkDetailViewModel : TimelineBaseViewModel<ResponsesResult>
    {
        public PlurkDetailHeaderViewModel ListHeader { get; private set; }

        public PlurkDetailViewModel
            (INavigationService navigationService,
            IProgressService progressService,
            IPlurkService plurkService,
            PlurkDetailHeaderViewModel plurkDetailHeaderViewModel)
            : base(navigationService, progressService, plurkService)
        {
            ListHeader = plurkDetailHeaderViewModel;
        }
    }
}
