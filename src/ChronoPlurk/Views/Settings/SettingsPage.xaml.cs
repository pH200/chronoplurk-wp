using System;
using WP7Contrib.View.Transitions.Animation;

namespace ChronoPlurk.Views.Settings
{
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            switch (animationType)
            {
                case AnimationType.NavigateBackwardOut:
                    return new SlideDownAnimator() { RootElement = LayoutRoot };
                case AnimationType.NavigateForwardIn:
                    return new SlideUpAnimator() { RootElement = LayoutRoot };
            }
            return base.GetAnimation(animationType, toOrFrom);
        }
    }
}
