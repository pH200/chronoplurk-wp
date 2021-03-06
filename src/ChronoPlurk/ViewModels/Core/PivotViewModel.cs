﻿using System;
using Caliburn.Micro;
using ChronoPlurk.Framework;

namespace ChronoPlurk.ViewModels
{
    public class PivotViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly PivotFix<IScreen> _pivotFix;

        public PivotViewModel()
        {
            _pivotFix = new PivotFix<IScreen>(this);
        }

        protected override void OnViewLoaded(object view)
        {
            _pivotFix.OnViewLoaded(view, base.OnViewLoaded);
        }

        protected override void ChangeActiveItem(IScreen newItem, bool closePrevious)
        {
            _pivotFix.ChangeActiveItem(newItem, closePrevious, base.ChangeActiveItem);
        }
    }
}
