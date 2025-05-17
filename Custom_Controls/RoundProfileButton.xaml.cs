using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LynxUI_Main.Custom_Controls
{
    public partial class RoundProfileButton : UserControl
    {
        public RoundProfileButton()
        {
            InitializeComponent();
        }

        // StrokeBrush DependencyProperty
        public static readonly DependencyProperty StrokeBrushProperty =
            DependencyProperty.Register(nameof(StrokeBrush), typeof(SolidColorBrush), typeof(RoundProfileButton));

        public SolidColorBrush StrokeBrush
        {
            get => (SolidColorBrush)GetValue(StrokeBrushProperty);
            set => SetValue(StrokeBrushProperty, value);
        }

        // IsOnline DependencyProperty
        public static readonly DependencyProperty IsOnlineProperty =
            DependencyProperty.Register(nameof(IsOnline), typeof(bool), typeof(RoundProfileButton));

        public bool IsOnline
        {
            get => (bool)GetValue(IsOnlineProperty);
            set => SetValue(IsOnlineProperty, value);
        }

        // ProfileImageSource DependencyProperty
        public static readonly DependencyProperty ProfileImageSourceProperty =
            DependencyProperty.Register(nameof(ProfileImageSource), typeof(ImageSource), typeof(RoundProfileButton));

        public ImageSource ProfileImageSource
        {
            get => (ImageSource)GetValue(ProfileImageSourceProperty);
            set => SetValue(ProfileImageSourceProperty, value);
        }
    }
}
