using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MetroPlurk.Views.PlurkControls
{
    public partial class PlurkItemControl : UserControl
    {
        public PlurkItemControl()
        {
            // Required to initialize variables
            InitializeComponent();

            MenuReply.Click += OnMenuReplyClick;
            MenuLike.Click += OnMenuLikeClick;
            MenuMute.Click += OnMenuMuteClick;
        }

        public event RoutedEventHandler MenuReplyClick;

        protected void OnMenuReplyClick(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler handler = MenuReplyClick;
            if (handler != null) handler(this, e);
        }

        public event RoutedEventHandler MenuLikeClick;

        public void OnMenuLikeClick(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler handler = MenuLikeClick;
            if (handler != null) handler(this, e);
        }

        public event RoutedEventHandler MenuMuteClick;

        public void OnMenuMuteClick(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler handler = MenuMuteClick;
            if (handler != null) handler(this, e);
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

        #region ContextMenuEnabled (DependencyProperty)

        /// <summary>
        /// Context menu availability.
        /// </summary>
        public bool ContextMenuEnabled
        {
            get { return (bool)GetValue(ContextMenuEnabledProperty); }
            set { SetValue(ContextMenuEnabledProperty, value); }
        }
        public static readonly DependencyProperty ContextMenuEnabledProperty =
            DependencyProperty.Register("ContextMenuEnabled", typeof(bool), typeof(PlurkItemControl),
            new PropertyMetadata(true, new PropertyChangedCallback(OnContextMenuEnabledChanged)));

        private static void OnContextMenuEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlurkItemControl)d).OnContextMenuEnabledChanged(e);
        }

        protected virtual void OnContextMenuEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            Menu.IsEnabled = (bool) e.NewValue;
        }

        #endregion
    }
}