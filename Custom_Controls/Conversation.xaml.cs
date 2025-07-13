using LynxUI_Main.ViewModels;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            this.Loaded += Conversation_Loaded;
        }


        /*private static void OnUserIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Conversation control && e.NewValue is int userId)
            {
                var viewModel = new ConversationViewModel(userId);

                if (control.SelectedChat != null)
                    viewModel.ActiveChat = control.SelectedChat;

                control.DataContext = viewModel;
            }
        }*/

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

        private void Conversation_Loaded(object sender, RoutedEventArgs e)
        {
            var dc = this.DataContext;
            if (dc != null)
            {
                var prop = dc.GetType().GetProperty("Messages");
                if (prop != null && prop.GetValue(dc) is INotifyCollectionChanged observable)
                {
                    observable.CollectionChanged += (s, ev) =>
                    {
                        ScrollToBottom();
                    };
                }
            }

            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(this);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToEnd();
            }
        }

        private void BtnSendImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Multiselect = false
            };
            if (dlg.ShowDialog() == true)
            {
                // Gọi ViewModel để gửi ảnh
                if (DataContext is ConversationViewModel vm)
                {
                    vm.SendImage(dlg.FileName);
                }
            }
        }

        private void BtnSendFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "All Files|*.*",
                Multiselect = false
            };
            if (dlg.ShowDialog() == true)
            {
                // Gọi ViewModel để gửi file
                if (DataContext is ConversationViewModel vm)
                {
                    vm.SendFile(dlg.FileName);
                }
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("[ERROR] Image failed to load: " + e.ErrorException?.Message);
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t)
                    return t;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
    }
}


