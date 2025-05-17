using LynxUI_Main.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media;
using LynxUI_Main.Models;

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
        private void SendImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Filter = "Các tệp hình ảnh|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Chọn hình ảnh để gửi",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFile = dialog.FileName;
                if (DataContext is ConversationViewModel vm)
                {
                    vm.SendImage(selectedFile);
                }
            }
        }
        private void SendFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Tất cả các tệp|*.*",
                Title = "Chọn tệp để gửi",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFile = dialog.FileName;
                if (DataContext is LynxUI_Main.ViewModels.ConversationViewModel vm)
                {
                    vm.SendFile(selectedFile);
                }
            }
        }
        private void PlayVideoButton_Click(object sender, RoutedEventArgs e)
        {
            // Tìm MediaElement cùng Grid với nút Play
            if (sender is Button btn &&
                VisualTreeHelper.GetParent(btn) is Grid grid)
            {
                foreach (var child in grid.Children)
                {
                    if (child is MediaElement media)
                    {
                        media.Position = TimeSpan.Zero; // Phát lại từ đầu nếu muốn
                        media.Play();
                        break;
                    }
                }
            }
        }



    }
}
