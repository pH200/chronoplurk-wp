using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Caliburn.Micro;
using MetroPlurk.Helpers;
using MetroPlurk.Services;
using MetroPlurk.ViewModels;
using Ninject;

namespace MetroPlurk
{
    public class AppBootstrapper : PhoneBootstrapper
    {
        private IKernel _kernel;

        protected override void Configure()
        {
            DefaultConfiguration.Initialize();

            _kernel = new StandardKernel();
     
            _kernel.Bind(typeof(MainPageViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(SearchResultViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(SearchRecordsViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(PlurkMainPageViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(TimelineViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(SearchPageViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(PlurkDetailPageViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(PlurkDetailViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(PlurkDetailHeaderViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(PlurkDetailFooterViewModel)).ToSelf().InSingletonScope();
            _kernel.Bind(typeof(ComposePageViewModel)).ToSelf().InSingletonScope();

            _kernel.Bind<IProgressService>().To<ProgressService>().InSingletonScope();
            _kernel.Bind<IPlurkService>().To<PlurkService>().InSingletonScope();

            _kernel.Bind<LoginViewModel>().ToSelf();

            _kernel.Bind<INavigationService>().ToConstant(new SpecialFrameAdapter(RootFrame));
            _kernel.Bind<IPhoneService>().ToConstant(new PhoneApplicationServiceAdapter(RootFrame));

            //container.Activator.InstallChooser<PhoneNumberChooserTask, PhoneNumberResult>();
            //container.Activator.InstallLauncher<EmailComposeTask>();

            AddCustomConventions();

            AddPhoneResources();

            AddNavigatingControl();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service != null)
            {
                return _kernel.Get(service, key);
            }

            throw new ArgumentNullException("service");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            _kernel.Inject(instance);
        }

        static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, viewModelType);
                        return true;
                    }

                    return false;
                };
            ConventionManager.AddElementConvention<Panorama>(Panorama.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, viewModelType);
                        return true;
                    }

                    return false;
                };
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
