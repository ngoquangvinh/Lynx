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
