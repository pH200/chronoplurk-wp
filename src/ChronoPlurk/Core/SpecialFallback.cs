using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ChronoPlurk.Core
{
    public sealed class SpecialFallback<T>
    {
        public Func<T, bool> Predicate { get; set; }

        public Action Fallback { get; set; }

        public SpecialFallback(Func<T, bool> predicate, Action fallback)
        {
            Predicate = predicate;
            Fallback = fallback;
        }
    }
}
