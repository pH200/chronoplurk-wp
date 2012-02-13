using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Autofac;
using Caliburn.Micro;
using ChronoPlurk.Core;
using ChronoPlurk.Helpers;
using ChronoPlurk.Services;
using ChronoPlurk.ViewModels;
using ChronoPlurk.ViewModels.Compose;
using ChronoPlurk.ViewModels.FriendsFans;
using ChronoPlurk.ViewModels.Main;
using ChronoPlurk.ViewModels.Profile;
using ChronoPlurk.ViewModels.Settings;
using ChronoPlurk.Views.ImageLoader;
using Microsoft.Phone.Controls;

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
            EmoticonsDictionary.Initialize();
            
            base.Configure();

            var plurkService = Container.Resolve<IPlurkService>();
            if (plurkService.LoadUserData())
            {
                var emoticonService = Container.Resolve<RecentEmoticonsService>();
                emoticonService.Load(plurkService.UserId);
            }

            AddConventions();

            AddPhoneResources();

            AddNavigatingControl();

            InitializeSettings();
        }

        private static void AddConventions()
        {
            ConventionManager.AddElementConvention<MenuItem>(ItemsControl.ItemsSourceProperty, "DataContext", "Click");
        }

        private void InitializeSettings()
        {
            var settings = Container.Resolve<SettingsService>();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            builder.RegisterType(typeof(MainPageViewModel)).AsSelf().SingleInstance();
            builder.RegisterType(typeof(PlurkMainPageViewModel)).AsSelf().SingleInstance();
            
            builder.RegisterType<SearchResultViewModel>().AsSelf();
            builder.RegisterType<SearchRecordsViewModel>().AsSelf();
            builder.RegisterType<SearchPageViewModel>().AsSelf();
            builder.RegisterType<PlurkDetailPageViewModel>().AsSelf();
            builder.RegisterType<PlurkDetailViewModel>().AsSelf();
            builder.RegisterType<PlurkDetailHeaderViewModel>().AsSelf();
            builder.RegisterType<PlurkDetailFooterViewModel>().AsSelf();
            builder.RegisterType<PlurkDetailReplyViewModel>().AsSelf();
            builder.RegisterType<SettingsPageViewModel>().AsSelf();
            
            #region Main VM
            builder.RegisterType<TimelineViewModel>().AsSelf();
            builder.RegisterType<UnreadPlurksViewModel>().AsSelf();
            builder.RegisterType<MyPlurksViewModel>().AsSelf();
            builder.RegisterType<PrivatePlurksViewModel>().AsSelf();
            builder.RegisterType<RespondedPlurksViewModel>().AsSelf();
            builder.RegisterType<LikedPlurksViewModel>().AsSelf();
            #endregion

            #region Compose VM
            builder.RegisterType<ComposePageViewModel>().AsSelf();
            builder.RegisterType<FriendsSelectionPageViewModel>().AsSelf();
            #endregion

            #region FriendsFans VM

            builder.RegisterType<FriendsFansListPageViewModel>().AsSelf();
            builder.RegisterType<PeopleListViewModel>().AsSelf();

            #endregion

            builder.RegisterType<PlurkProfilePageViewModel>().AsSelf();
            builder.RegisterType<ProfileTimelineViewModel>().AsSelf();

            builder.RegisterType<LoginPageViewModel>().AsSelf();

            #region Application Services
            builder.Register(c => new ProgressService()).As(typeof(IProgressService)).SingleInstance();
            builder.Register(c => new AutoRotateService()).AsSelf().SingleInstance();
            builder.RegisterType<SettingsService>().AsSelf().SingleInstance();
            builder.Register(c => new PlurkHolderService()).AsSelf().SingleInstance();
            builder.RegisterType<FriendsFansCompletionService>().AsSelf().SingleInstance();
            builder.Register(c => new RecentEmoticonsService()).AsSelf().SingleInstance();
            #endregion

            #region Plurk Services
            builder.RegisterInstance(new PlurkContentStorageService()).As(typeof(IPlurkContentStorageService)).SingleInstance();
            builder.RegisterType(typeof(PlurkService)).As(typeof(IPlurkService)).SingleInstance();
            #endregion

            builder.RegisterType<SettingsOssCreditsPageViewModel>().AsSelf().InstancePerDependency();
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
