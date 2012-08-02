using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChronoPlurk.Helpers;
using ChronoPlurk.Models;

namespace ChronoPlurk.Services
{
    public class BackgroundImageService
    {
        public bool IsEnabled { get; set; }

        public double Opacity { get; set; }

        private ImageBrush _cachedBg;

        public BackgroundImageService()
        {
            IsEnabled = true;
            Opacity = 0.2;
        }

        public void LoadSettings(BgSettings settings)
        {
            IsEnabled = settings.IsEnabled;
            Opacity = settings.Opacity;
        }

        public BgSettings ExportSettings()
        {
            return new BgSettings()
            {
                IsEnabled = IsEnabled,
                Opacity = Opacity,
            };
        }

        public void ApplyBackground(object page)
        {
            var layoutRoot = GetLayoutRoot(page);
            if (layoutRoot != null)
            {
                if (IsEnabled)
                {
                    if (_cachedBg == null)
                    {
                        LoadCachedBg();
                    }
                    layoutRoot.Background = _cachedBg;
                }
                else
                {
                    layoutRoot.Background = null;
                }
            }
        }

        private void LoadCachedBg()
        {
            var image = PlurkResources.IsLightTheme
                                ? "/Resources/Images/bg_white.jpg"
                                : "/Resources/Images/bg_black.jpg";
            _cachedBg = new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(image, UriKind.Relative)),
                Stretch = Stretch.UniformToFill,
                Opacity = Opacity,
            };
        }

        public void ChangeEnabled(object page, bool value)
        {
            IsEnabled = value;
            var layoutRoot = GetLayoutRoot(page);
            if (layoutRoot != null)
            {
                if (IsEnabled)
                {
                    if (!(layoutRoot.Background is ImageBrush))
                    {
                        ApplyBackground(page);
                    }
                }
                else
                {
                    layoutRoot.Background = null;
                }
            }
        }

        public void ChangeOpacity(object page, double opacity)
        {
            Opacity = opacity;
            _cachedBg.Opacity = opacity;
        }

        protected static Grid GetLayoutRoot(object page)
        {
            var view = page as DependencyObject;
            if (view != null)
            {
                var layoutRoot = VisualTreeHelper.GetChild(view, 0) as Grid;
                if (layoutRoot != null && layoutRoot.Name == "LayoutRoot")
                {
                    return layoutRoot;
                }
            }
            return null;
        }
    }
}
