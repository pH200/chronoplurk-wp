using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChronoPlurk.Helpers;
using ChronoPlurk.Models;
using ChronoPlurk.Resources.i18n;
using Microsoft.Phone;
using Microsoft.Phone.Tasks;

namespace ChronoPlurk.Services
{
    public class BackgroundImageService
    {
        public const string CustomBgPath = "bgs/user.jpg";

        public bool IsEnabled { get; set; }

        public double Opacity { get; set; }
        
        public string SelectedPath { get; set; }

        public bool IsDefaultBg
        {
            get { return SelectedPath == null || SelectedPath == "Default"; }
        }

        private ImageBrush _cachedBg;

        private IDisposable _photoChooserDisposable;

        public BackgroundImageService()
        {
            IsEnabled = true;
            Opacity = 0.2;
            SelectedPath = "Default";
        }

        public void LoadSettings(BgSettings settings)
        {
            IsEnabled = settings.IsEnabled;
            Opacity = settings.Opacity;
            SelectedPath = settings.SelectedPath;
        }

        public BgSettings ExportSettings()
        {
            return new BgSettings()
            {
                IsEnabled = IsEnabled,
                Opacity = Opacity,
                SelectedPath = SelectedPath,
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
            Action createDefaultBg = () =>
            {
                var imagePath = PlurkResources.IsLightTheme
                                    ? "/Resources/Images/bg_white.jpg"
                                    : "/Resources/Images/bg_black.jpg";
                _cachedBg = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Relative)),
                    Stretch = Stretch.UniformToFill,
                    Opacity = Opacity,
                };
            };
            if (IsDefaultBg)
            {
                createDefaultBg();
            }
            else
            {
                using (var memoryStream = IsoSettings.ReadBackgroundFile(SelectedPath))
                {
                    if (memoryStream == null)
                    {
                        createDefaultBg();
                    }
                    else
                    {
                        try
                        {
                            var bm = new BitmapImage();
                            bm.SetSource(memoryStream); // BitmapImage doesn't support isostore uri.
                            _cachedBg = new ImageBrush()
                            {
                                ImageSource = bm,
                                Stretch = Stretch.UniformToFill,
                                Opacity = Opacity,
                            };
                        }
                        catch (Exception)
                        {
                            createDefaultBg();
                        }
                    }
                }
            }
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

        public void ChangeBgDefault()
        {
            ChangeBg("Default");
        }

        public void ChangeBg(string path)
        {
            SelectedPath = path;
            LoadCachedBg();
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

        #region Photo Chooser

        /// <summary>
        /// Pop-up photo chooser, must call from UI thread.
        /// </summary>
        public void SelectFromPhotoChooser()
        {
            ShowPhotoChooser();
        }

        private void ShowPhotoChooser()
        {
            try
            {
                if (_photoChooserDisposable != null)
                {
                    _photoChooserDisposable.Dispose();
                }
                var photoChooser = new PhotoChooserTask() {ShowCamera = true};
                var pattern = Observable.FromEventPattern<PhotoResult>(handler => photoChooser.Completed += handler,
                                                                       handler => photoChooser.Completed -= handler);
                _photoChooserDisposable = pattern.TimeInterval().Subscribe(result =>
                {
                    var photoResult = result.Value.EventArgs;
                    switch (photoResult.TaskResult)
                    {
                        case TaskResult.OK:
                            ProcessPhoto(photoResult.ChosenPhoto);
                            break;
                        case TaskResult.Cancel:
                            if (result.Interval < TimeSpan.FromSeconds(1.5))
                            {
                                ThreadEx.OnUIThread(() => MessageBox.Show(AppResources.msgDisconnectPC));
                            }
                            break;
                    }
                });
                photoChooser.Show();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(AppResources.errorOccured);
            }
        }

        private void ProcessPhoto(Stream pictureStream)
        {
            using (var photoStream = pictureStream)
            {
                // Resizing
                var bitmap = PictureDecoder.DecodeJpeg(photoStream, 800, 800);
                var filename = CustomBgPath;
                IsoSettings.CreateBackgroundFile(filename, fileStream =>
                {
                    bitmap.SaveJpeg(fileStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 80);
                });
                ChangeBg(filename);
            }
        }

        #endregion

    }
}
