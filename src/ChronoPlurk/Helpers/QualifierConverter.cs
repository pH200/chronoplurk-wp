﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Plurto.Core;
using Plurto.Entities;

namespace ChronoPlurk.Helpers
{
    public static class QualifierConverter
    {
        public static SolidColorBrush ConvertQualifierBrush(Qualifier qualifier)
        {
            return QualifierBrushes.Brushes[qualifier];
        }

        public static Color ConvertQualifierColor(Qualifier qualifier)
        {
            switch (qualifier)
            {
                case Qualifier.Asks: return QualifierColors.Asks;
                case Qualifier.Feels: return QualifierColors.Feels;
                case Qualifier.Gives: return QualifierColors.Gives;
                case Qualifier.Has: return QualifierColors.Has;
                case Qualifier.Hates: return QualifierColors.Hates;
                case Qualifier.Hopes: return QualifierColors.Hopes;
                case Qualifier.Is: return QualifierColors.Is;
                case Qualifier.Likes: return QualifierColors.Likes;
                case Qualifier.Loves: return QualifierColors.Loves;
                case Qualifier.Needs: return QualifierColors.Needs;
                case Qualifier.Says: return QualifierColors.Says;
                case Qualifier.Shares: return QualifierColors.Shares;
                case Qualifier.Thinks: return QualifierColors.Thinks;
                case Qualifier.Wants: return QualifierColors.Wants;
                case Qualifier.Was: return QualifierColors.Was;
                case Qualifier.Will: return QualifierColors.Will;
                case Qualifier.Wishes: return QualifierColors.Wishes;
                case Qualifier.Wonders: return QualifierColors.Wonders;
                case Qualifier.Freestyle: return QualifierColors.Freestyle;
                case Qualifier.FreestyleColon: return QualifierColors.Freestyle;
            }
            return default(Color);
        }

        public static class QualifierColors
        {
            public static readonly Color Freestyle = Color.FromArgb(255, 238, 238, 238);
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

        public static class QualifierBrushes
        {
            public static IDictionary<Qualifier, SolidColorBrush> Brushes { get; private set; }
            
            static QualifierBrushes()
            {
                Brushes =
                    GetEnumValues<Qualifier>().
                        ToDictionary(q => q, q => new SolidColorBrush(QualifierConverter.ConvertQualifierColor(q)));
            }

            private static T[] GetEnumValues<T>()
            {
                var type = typeof(T);
                if (!type.IsEnum)
                    throw new ArgumentException("Type '" + type.Name + "' is not an enum");

                return (from field in type.GetFields(BindingFlags.Public | BindingFlags.Static)
                        where field.IsLiteral
                        let en = field.GetValue(type)
                        orderby (int)en
                        select (T)en).ToArray();
            }
        }
    }

    public static class PlurkExtensions
    {
        public static string QualifierTextView(this Plurk plurk)
        {
            if (plurk.Qualifier == Qualifier.Freestyle || plurk.Qualifier == Qualifier.FreestyleColon)
            {
                return null;
            }
            return !String.IsNullOrEmpty(plurk.QualifierTranslated) ? plurk.QualifierTranslated : plurk.Qualifier.ToKey();
        }
    }
}
