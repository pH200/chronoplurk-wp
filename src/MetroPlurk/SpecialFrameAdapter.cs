using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Caliburn.Micro;

namespace MetroPlurk
{
    public class SpecialFrameAdapter : FrameAdapter
    {
        public SpecialFrameAdapter(Frame frame, bool treatViewAsLoaded = false)
            : base(frame, treatViewAsLoaded)
        {
        }

        internal struct NavigationHelper
        {
            public Uri TargetUri { get; set; }
            public Uri CurrentUri { get; set; }
            public NavigationMode NavMode { get; set; }
        }
    }
}
