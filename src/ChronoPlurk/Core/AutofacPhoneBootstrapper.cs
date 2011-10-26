using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Autofac;
using Caliburn.Micro;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Core
{
    public abstract class AutofacPhoneBootstrapper : PhoneBootstrapper
    {
        protected IContainer Container { get; private set; }

        #region Builders
        protected virtual Func<INavigationService> BuildFrameAdapter
        {
            get { return () => new FrameAdapter(RootFrame); }
        }

        protected virtual Func<IPhoneService> BuildPhoneService
        {
            get { return () => new PhoneApplicationServiceAdapter(RootFrame); }
        }

        protected virtual Func<IEventAggregator> BuildEventAggregator
        {
            get { return () => new EventAggregator(); }
        }

        protected virtual Func<IWindowManager> BuildWindowManager
        {
            get { return () => new WindowManager(); }
        }

        protected virtual Func<ISoundEffectPlayer> BuildSoundEffectPlayer
        {
            get { return () => new XnaSoundEffectPlayer(); }
        }

        protected virtual Func<IVibrateController> BuildVibrateController
        {
            get { return () => new SystemVibrateController(); }
        }
        #endregion

        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            // Frame adapter must be generated before first page navigation.
            builder.RegisterInstance(BuildFrameAdapter());
            builder.RegisterInstance(BuildPhoneService());

            builder.Register<IEventAggregator>(c => BuildEventAggregator()).SingleInstance();
            builder.Register<IWindowManager>(c => BuildWindowManager()).SingleInstance();
            builder.Register<ISoundEffectPlayer>(c => BuildSoundEffectPlayer()).SingleInstance();
            builder.Register<IVibrateController>(c => BuildVibrateController()).SingleInstance();
            builder.RegisterType<StorageCoordinator>().AsSelf().SingleInstance();
            builder.RegisterType<TaskController>().AsSelf().SingleInstance();

            ConfigureContainer(builder);

            Container = builder.Build();

            StartServices();

            AddCustomConventions();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service != null)
            {
                return Container.Resolve(service);
            }
            else
            {
                throw new ArgumentNullException("service");
            }
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
        }

        private void StartServices()
        {
            //Container.Resolve<StorageCoordinator>().Start();
            Container.Resolve<TaskController>().Start();
        }

        private static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<Pivot>
                (Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").
                ApplyBinding = (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager.GetElementConvention
                        (typeof(ItemsControl)).ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager.ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                        ConventionManager.ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, viewModelType);
                        return true;
                    }
                    return false;
                };

            ConventionManager.AddElementConvention<Panorama>
                (Panorama.ItemsSourceProperty, "SelectedItem", "SelectionChanged").
                ApplyBinding = (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager.GetElementConvention
                        (typeof(ItemsControl)).ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager.ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
                        ConventionManager.ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, viewModelType);
                        return true;
                    }
                    return false;
                };
        }
    }
}
