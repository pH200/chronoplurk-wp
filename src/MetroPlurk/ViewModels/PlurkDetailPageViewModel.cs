using System;
using System.Linq;
using System.Windows.Media;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using NotifyPropertyWeaver;
using Plurto.Core;

namespace MetroPlurk.ViewModels
{
    public class PlurkDetailPageViewModel : LoginAvailablePage, INavigationInjectionRedirect
    {
        private readonly IPlurkService _plurkService;
        
        public PlurkDetailViewModel PlurkDetailViewModel { get; private set; }
        
        public PlurkDetailPageViewModel
            (IPlurkService plurkService,
            PlurkDetailViewModel plurkDetailViewModel,
            LoginViewModel loginViewModel)
            : base(loginViewModel)
        {
            _plurkService = plurkService;
            PlurkDetailViewModel = plurkDetailViewModel;
        }

        public object GetRedirectedViewModel()
        {
            return PlurkDetailViewModel.ListHeader;
        }
    }
}
