using LynxUI_Main.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LynxUI_Main.Custom_Controls
{
    /// <summary>
    /// Interaction logic for Conversation.xaml
    /// </summary>
    public partial class Conversation : UserControl
    {
        public Conversation()
        {
            InitializeComponent();
            DataContext = new ConversationViewModel();
        }

        public static readonly DependencyProperty SelectedChatProperty =
        DependencyProperty.Register("SelectedChat", typeof(ChatListItem), typeof(Conversation), new PropertyMetadata(null));

        public ChatListItem SelectedChat
        {
            get => (ChatListItem)GetValue(SelectedChatProperty);
            set => SetValue(SelectedChatProperty, value);
        }

        private static void OnSelectedChatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Conversation;
            if (control?.DataContext is ConversationViewModel vm && e.NewValue is ChatListItem chat)
            {
                vm.ActiveChat = chat;
            }
        }

        private void BtnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ToggleSidebar(); // Gọi hàm ToggleSidebar trong MainWindow
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None && DataContext is ConversationViewModel vm)
            {
                vm.SendMessage();
                e.Handled = true; // Ngăn Enter xuống dòng
            }
        }
    }
}
