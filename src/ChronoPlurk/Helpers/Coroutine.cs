using System;
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
    }
}
