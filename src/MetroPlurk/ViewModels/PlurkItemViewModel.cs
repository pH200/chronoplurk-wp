using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MetroPlurk.Helpers;
using Plurto.Core;

namespace MetroPlurk.ViewModels
{
    public sealed class PlurkItemViewModel
    {
        public string UserName { get; set; }

        public Qualifier QualifierEnum { get; set; }

        public string Qualifier { get; set; }

        public Color QualifierColor { get { return ConvertQualifierColor(QualifierEnum); } }

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

        public static Color ConvertQualifierColor(Qualifier qualifier)
        {
            switch (qualifier)
            {
                case Plurto.Core.Qualifier.Asks:
                    return QualifierColors.Asks;
                case Plurto.Core.Qualifier.Feels:
                    return QualifierColors.Feels;
                case Plurto.Core.Qualifier.Gives:
                    return QualifierColors.Gives;
                case Plurto.Core.Qualifier.Has:
                    return QualifierColors.Has;
                case Plurto.Core.Qualifier.Hates:
                    return QualifierColors.Hates;
                case Plurto.Core.Qualifier.Hopes:
                    return QualifierColors.Hopes;
                case Plurto.Core.Qualifier.Is:
                    return QualifierColors.Is;
                case Plurto.Core.Qualifier.Likes:
                    return QualifierColors.Likes;
                case Plurto.Core.Qualifier.Loves:
                    return QualifierColors.Loves;
                case Plurto.Core.Qualifier.Needs:
                    return QualifierColors.Needs;
                case Plurto.Core.Qualifier.Says:
                    return QualifierColors.Says;
                case Plurto.Core.Qualifier.Shares:
                    return QualifierColors.Shares;
                case Plurto.Core.Qualifier.Thinks:
                    return QualifierColors.Thinks;
                case Plurto.Core.Qualifier.Wants:
                    return QualifierColors.Wants;
                case Plurto.Core.Qualifier.Was:
                    return QualifierColors.Was;
                case Plurto.Core.Qualifier.Will:
                    return QualifierColors.Will;
                case Plurto.Core.Qualifier.Wishes:
                    return QualifierColors.Wishes;
                case Plurto.Core.Qualifier.Wonders:
                    return QualifierColors.Wonders;
            }
            return default(Color);
        }

        public static class QualifierColors
        {
            public static readonly Color Is = Color.FromArgb(255, 229, 124, 67);
            public static readonly Color Says = Color.FromArgb(255, 226, 86, 11);
            public static readonly Color Needs = Color.FromArgb(255, 122, 154, 55);
            public static readonly Color Hopes = Color.FromArgb(255, 224, 91, 233);
            public static readonly Color Feels = Color.FromArgb(255, 45, 131, 190);
            public static readonly Color Thinks = Color.FromArgb(255, 104, 156, 193);
            public static readonly Color Wants = Color.FromArgb(255, 141, 178, 65);
            public static readonly Color Wishes = Color.FromArgb(255, 91, 176, 23);
            public static readonly Color Has = Color.FromArgb(255, 119, 119, 119);
            public static readonly Color Loves = Color.FromArgb(255, 178, 12, 12);
            public static readonly Color Hates = Color.FromArgb(255, 17, 17, 17);
            public static readonly Color Asks = Color.FromArgb(255, 131, 97, 188);
            public static readonly Color Will = Color.FromArgb(255, 180, 109, 185);
            public static readonly Color Was = Color.FromArgb(255, 82, 82, 82);
            public static readonly Color Had = Color.FromArgb(255, 140, 140, 140);
            public static readonly Color Likes = Color.FromArgb(255, 203, 39, 40);
            public static readonly Color Shares = Color.FromArgb(255, 167, 73, 73);
            public static readonly Color Gives = Color.FromArgb(255, 98, 14, 14);
            public static readonly Color Wonders = Color.FromArgb(255, 46, 78, 158);
        }
    }
}
