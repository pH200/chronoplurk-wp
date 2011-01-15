using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Caliburn.Micro;
using MetroPlurk.Framework;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using MetroPlurk.ViewModels;

namespace MetroPlurk
{
    public class AppBootstrapper : PhoneBootstrapper
    {
        PhoneContainer container;

        protected override void Configure()
        {
            DefaultConfiguration.Initialize();

            container = new PhoneContainer(this);

            container.RegisterSingleton(typeof(MainPageViewModel), "MainPageViewModel", typeof(MainPageViewModel));
            container.RegisterSingleton(typeof(SearchResultViewModel), null, typeof(SearchResultViewModel));
            container.RegisterSingleton(typeof(SearchRecordsViewModel), null, typeof(SearchRecordsViewModel));
            container.RegisterSingleton(typeof(PlurkMainPageViewModel), "PlurkMainPageViewModel", typeof(PlurkMainPageViewModel));
            container.RegisterSingleton(typeof(TimelineViewModel), null, typeof(TimelineViewModel));
            container.RegisterSingleton(typeof(SearchPageViewModel), "SearchPageViewModel", typeof(SearchPageViewModel));
            container.RegisterSingleton(typeof(IProgressService), null, typeof(ProgressService));
            container.RegisterSingleton(typeof(IPlurkService), null, typeof(PlurkService));
            
            container.RegisterPerRequest(typeof(LoginViewModel), null, typeof(LoginViewModel));

            container.RegisterInstance(typeof(INavigationService), null, new SpecialFrameAdapter(RootFrame));
            container.RegisterInstance(typeof(IPhoneService), null, new PhoneApplicationServiceAdapter(PhoneService));

            //container.Activator.InstallChooser<PhoneNumberChooserTask, PhoneNumberResult>();
            //container.Activator.InstallLauncher<EmailComposeTask>();

            AddCustomConventions();

            AddPhoneResources();

            AddNavigatingControl();

#if CLEAN_DEBUG
                // Clear settings for debugging
                IsolatedStorageSettings.ApplicationSettings.Clear();
#endif
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention);
                    ConventionManager
                        .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                    ConventionManager
                        .ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, viewModelType);
                };

            ConventionManager.AddElementConvention<Panorama>(Panorama.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention);
                    ConventionManager
                        .ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
                    ConventionManager
                        .ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, viewModelType);
                };
        }

        private void AddPhoneResources()
        {
            PlurkResources.PhoneForegroundBrush = (SolidColorBrush)Application.Resources["PhoneForegroundBrush"];
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

                if (IsoSettings.Settings.Contains(AppUserInfo.StorageKey) &&
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
