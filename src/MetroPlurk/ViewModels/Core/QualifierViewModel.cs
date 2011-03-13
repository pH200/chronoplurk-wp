using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MetroPlurk.Helpers;
using NotifyPropertyWeaver;
using Plurto.Core;

namespace MetroPlurk.ViewModels
{
    [NotifyForAll]
    public class QualifierViewModel
    {
        public Qualifier Qualifier { get; set; }

        [DependsOn("Qualifier")]
        public string Text { get { return Qualifier.ToKey(); } }

        [DependsOn("Qualifier")]
        public SolidColorBrush Brush { get { return QualifierConverter.ConvertQualifierBrush(Qualifier); } }

        private static QualifierViewModel[] _allQualifiers;
        public static IList<QualifierViewModel> AllQualifiers
        {
            get
            {
                return _allQualifiers ??
                       (_allQualifiers =
                        GetEnumValues<Qualifier>().Where(q => q != Qualifier.Freestyle).Select(
                            q => new QualifierViewModel(q)).ToArray());
            }
        }

        public QualifierViewModel()
        {
        }

        public QualifierViewModel(Qualifier qualifier)
        {
            Qualifier = qualifier;
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
