using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChronoPlurk.Views.PlurkControls
{
    public partial class PlurkItemControl : UserControl
    {
        public PlurkItemControl()
        {
            // Required to initialize variables
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
            DependencyProperty.Register("QualifierColor", typeof(Color), typeof(PlurkItemControl),
            new PropertyMetadata(Colors.LightGray, new PropertyChangedCallback(OnQualifierColorChanged)));

        private static void OnQualifierColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlurkItemControl)d).OnQualifierColorChanged(e);
        }

        protected virtual void OnQualifierColorChanged(DependencyPropertyChangedEventArgs e)
        {
            QualifierColorBrush.Color = (Color) e.NewValue;
        }

        #endregion
    }
}
