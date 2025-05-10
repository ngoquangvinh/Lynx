using LynxUI_Main.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LynxUI_Main.Custom_Controls
{
    /// <summary>
    /// Interaction logic for ChatList.xaml
    /// </summary>
    public partial class ChatList : UserControl
    {
        public event EventHandler<ChatListItem> ChatSelected;
        public ChatList()
        {
            InitializeComponent();
            DataContext = new ChatListViewModel();
        }

        private void ChatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChatListBox.SelectedItem is ChatListItem selectedChat)
            {
                ChatSelected?.Invoke(this, selectedChat);
            }
        }
        private void ChatListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(ChatListBox);
            if (scrollViewer != null)
            {
                // Mỗi dòng cao ~90 => 3 dòng ≈ 270
                double offsetChange = e.Delta > 0 ? -270 : 270;
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offsetChange);
                e.Handled = true;
            }
        }

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T correctlyTyped)
                    return correctlyTyped;

                var descendent = FindVisualChild<T>(child);
                if (descendent != null)
                    return descendent;
            }
            return null;
        }


    }
}
