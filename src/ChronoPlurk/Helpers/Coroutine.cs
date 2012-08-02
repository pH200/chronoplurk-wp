﻿using System;
using System.Threading;

namespace ChronoPlurk.Helpers
{
    public static class ThreadEx
    {
        public static void OnUIThread(this Action action)
        {
            Caliburn.Micro.Execute.OnUIThread(action);
        }

        public static void OnThreadPool(this Action action)
        {
            ThreadPool.QueueUserWorkItem(state => action());
        }

        public static void OnThreadPool(this Action action, Action onCompleteUI)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                action();
                onCompleteUI.OnUIThread();
            });
        }

        public static void OnThreadPool<T>(this Func<T> action, Action<T> onCompleteUI)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                var result = action();
                OnUIThread(() => onCompleteUI(result));
            });
        }
    }
}
