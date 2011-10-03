using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Phone.Controls;

namespace ChronoPlurk.Views.PlurkControls
{
    public class OrientationTrigger : TriggerAction<PhoneApplicationPage>
    {
        #region Visual Properties
        public string LandscapeStateName { get; set; }
        public string PortraitStateName { get; set; }
        #endregion

        #region Attach Method Overides
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_Loaded);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= new RoutedEventHandler(AssociatedObject_Loaded);
            base.OnDetaching();
        }
        #endregion

        #region UI Event Handler
        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateOrientationState(AssociatedObject.Orientation);
        }
        #endregion

        #region TriggerAction.Invoke
        protected override void Invoke(object parameter)
        {
            var ev = parameter as OrientationChangedEventArgs;
            UpdateOrientationState(ev.Orientation);
        }
        #endregion

        #region Members
        private void UpdateOrientationState(PageOrientation orientation)
        {
            string state = null;
            if (orientation == PageOrientation.Landscape || orientation == PageOrientation.LandscapeLeft || orientation == PageOrientation.LandscapeRight)
            {
                state = LandscapeStateName;
            }
            else
            {
                state = PortraitStateName;
            }
            VisualStateManager.GoToState(AssociatedObject, state, true);
        }
        #endregion
    }
}
