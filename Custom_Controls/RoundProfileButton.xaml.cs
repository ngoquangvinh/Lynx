using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LynxUI_Main.Custom_Controls
{
    public partial class RoundProfileButton : UserControl
    {
        // ✅ RoutedEvent để WPF hiểu Click="..."
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoundProfileButton));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public RoundProfileButton()
        {
            InitializeComponent();
            PART_Button.Click += (s, e) =>
            {
                RaiseEvent(new RoutedEventArgs(ClickEvent));
            };
        }

        public static readonly DependencyProperty StrokeBrushProperty =
            DependencyProperty.Register(nameof(StrokeBrush), typeof(SolidColorBrush), typeof(RoundProfileButton));

        public SolidColorBrush StrokeBrush
        {
            get => (SolidColorBrush)GetValue(StrokeBrushProperty);
            set => SetValue(StrokeBrushProperty, value);
        }

        public static readonly DependencyProperty IsOnlineProperty =
            DependencyProperty.Register(nameof(IsOnline), typeof(bool), typeof(RoundProfileButton));

        public bool IsOnline
        {
            get => (bool)GetValue(IsOnlineProperty);
            set => SetValue(IsOnlineProperty, value);
        }

        public static readonly DependencyProperty ProfileImageSourceProperty =
            DependencyProperty.Register(
                nameof(ProfileImageSource),
                typeof(ImageSource),
                typeof(RoundProfileButton),
                new PropertyMetadata(null));

        public ImageSource ProfileImageSource
        {
            get => (ImageSource)GetValue(ProfileImageSourceProperty);
            set => SetValue(ProfileImageSourceProperty, value);
        }
    }
}
