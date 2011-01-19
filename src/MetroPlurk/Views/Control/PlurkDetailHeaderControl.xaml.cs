using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MetroPlurk
{
    public partial class PlurkDetailHeaderControl : UserControl
    {
        public PlurkDetailHeaderControl()
        {
            InitializeComponent();
        }

        #region QualifierColor (DependencyProperty)

        /// <summary>
        /// Background color for qualifier.
        /// </summary>
        public Color QualifierColor
        {
            get { return (Color)GetValue(QualifierColorProperty); }
            set { SetValue(QualifierColorProperty, value); }
        }
        public static readonly DependencyProperty QualifierColorProperty =
            DependencyProperty.Register("QualifierColor", typeof(Color), typeof(PlurkDetailHeaderControl),
            new PropertyMetadata(Colors.LightGray, new PropertyChangedCallback(OnQualifierColorChanged)));

        private static void OnQualifierColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlurkDetailHeaderControl)d).OnQualifierColorChanged(e);
        }

        protected virtual void OnQualifierColorChanged(DependencyPropertyChangedEventArgs e)
        {
            QualifierColorBrush.Color = (Color)e.NewValue;
        }

        #endregion
    }
}
