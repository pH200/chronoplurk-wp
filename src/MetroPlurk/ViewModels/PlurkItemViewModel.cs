using System;
using System.Windows;
using System.Windows.Media;
using MetroPlurk.Helpers;
using Plurto.Core;

namespace MetroPlurk.ViewModels
{
    public sealed class PlurkItemViewModel
    {
        public string UserName { get; set; }

        public Qualifier QualifierEnum { get; set; }

        public string Qualifier { get; set; }

        public Color QualifierColor
        {
            get { return QualifierConverter.ConvertQualifierColor(QualifierEnum); }
        }

        public TimeSpan PostTimeFromNow { get; set; }

        public DateTime PostDate { get; set; }

        public string TimeView { get { return ConvertTimeSpan(PostTimeFromNow, PostDate); } }

        public string ContentRaw { get; set; }

        public string AvatarView { get; set; }

        public bool IsLike { get; set; }

        public SolidColorBrush FavoriteColorView
        {
            get
            {
                if (IsLike)
                {
                    return PlurkResources.PlurkColorBrightBrush;
                }
                return null;
            }
        }

        public int ResponseCount { get; set; }

        public string ResponseCountView
        {
            get
            {
                if (ResponseCount == 1)
                {
                    return "1 response";
                }
                if (ResponseCount > 1)
                {
                    return ResponseCount + " responses";
                }
                return string.Empty;
            }
        }

        public int ResponsesSeen { get; set; }

        public Visibility IsAllResponsesSeen
        {
            get { return ResponseCount == ResponsesSeen ? Visibility.Collapsed : Visibility.Visible; }
        }

        public bool ContextMenuEnabled { get; set; }
        
        public static string ConvertTimeSpan(TimeSpan timeSpan, DateTime? postDate = null)
        {
            if (timeSpan.TotalSeconds <= 1)
            {
                //return String.Format("{0} sec", (int)timeSpan.TotalSeconds);
                //return string.Empty;
                return "just a moment ago";
            }
            if (timeSpan.TotalSeconds < 120)
            {
                return String.Format("{0} secs ago", (int)timeSpan.TotalSeconds);
            }
            if (timeSpan.TotalHours < 2)
            {
                return String.Format("{0} mins ago", (int)timeSpan.TotalMinutes);
            }
            if (timeSpan.TotalDays < 2)
            {
                return String.Format("{0} hours ago", (int)timeSpan.TotalHours);
            }
            if (postDate == null)
            {
                postDate = DateTime.Now - timeSpan;
            }
            return postDate.Value.ToString("MMM d");
        }
    }
}
