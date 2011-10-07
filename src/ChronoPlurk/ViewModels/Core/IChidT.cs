using Caliburn.Micro;

namespace ChronoPlurk.ViewModels
{
    public interface IChildT<T> : IChild where T : class
    {
    }

    public static class ChildTExtensions
    {
        public static T GetParent<T>(this IChildT<T> viewModel)
            where T : class
        {
            return viewModel.Parent as T;
        }
    }
}
