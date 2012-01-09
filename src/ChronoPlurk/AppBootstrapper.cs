using System;
using System.Windows.Media;
using System.Windows.Navigation;
using Autofac;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Helpers;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels;
using ChronoPlurk.ViewModels.Settings;

namespace ChronoPlurk
{
    public class AppBootstrapper : AutofacPhoneBootstrapper
    {
        protected override Func<INavigationService> BuildFrameAdapter
        {
            get
            {
                return () => new SpecialFrameAdapter(RootFrame);
            }
        }

        protected override void Configure()
        {
            DefaultConfiguration.Initialize();
            
            base.Configure();

            AddPhoneResources();

            AddNavigatingControl();

            InitializeSettings();
        }

        private void InitializeSettings()
        {
            var settings = Container.Resolve<SettingsService>();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            builder.RegisterType(typeof(MainPageViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(SearchResultViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(SearchRecordsViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(PlurkMainPageViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(TimelineViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(SearchPageViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(PlurkDetailPageViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(PlurkDetailViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(PlurkDetailHeaderViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(PlurkDetailFooterViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(ComposePageViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(SettingsPageViewModel)).AsSelf().SingleInstance();

            builder.RegisterType<LoginPageViewModel>().AsSelf();

            #region Application Services
            builder.Register(c => new ProgressService()).As(typeof(IProgressService)).SingleInstance();
            builder.Register(c => new AutoRotateService()).AsSelf().SingleInstance();
            builder.RegisterType<SettingsService>().AsSelf().SingleInstance();
            builder.Register(c => new PlurkHolderService()).AsSelf().SingleInstance();
            #endregion

            #region Plurk Services
            builder.RegisterInstance(new PlurkContentStorageService()).As(typeof(IPlurkContentStorageService)).SingleInstance();
            builder.RegisterType(typeof(PlurkService)).As(typeof(IPlurkService)).SingleInstance();
            #endregion

            builder.RegisterType<SettingsOssCreditsPageViewModel>().AsSelf().InstancePerDependency();

            builder.RegisterType<LoginViewModel>().AsSelf().InstancePerDependency();
        }

        private void AddPhoneResources()
        {
            PlurkResources.PhoneForegroundBrush = (SolidColorBrush)Application.Resources["PhoneForegroundBrush"];
            PlurkResources.PhoneAccentBrush = (SolidColorBrush)Application.Resources["PhoneAccentBrush"];
            PlurkResources.PhoneWidthGetter = () => RootFrame.ActualWidth;
            PlurkResources.PhoneHeightGetter = () => RootFrame.ActualHeight;
        }

        private void AddNavigatingControl()
        {
            RootFrame.Navigating += (sender, e) =>
            {
                // Only care about MainPage
                if (!e.Uri.ToString().Contains("/MainPage.xaml"))
                {
                    return;
                }

                if (Container.Resolve<IPlurkService>().IsLoaded &&
                    e.NavigationMode != NavigationMode.Back)
                {
                    e.Cancel = true;

                    RootFrame.Dispatcher.BeginInvoke(() =>
                        RootFrame.Navigate(new Uri("//Views/PlurkMainPage.xaml", UriKind.Relative)));
                }
            };
        }
    }
}
